using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using System.Formats.Tar;
using System.Linq;

namespace Core
{
    public class HostConnection : Connection
    {
        public List<ConnectedPlayer> ConnectedPlayers = new();
        private Dictionary<IPEndPoint, ConnectingClient> connectingClients = new();
        private Dictionary<int, Schema.UpdatePacket> UpdatePackets = new();
        private int CurrentVersion = 0;

        struct ConnectingClient
        {
            public PlayerDetails Player;
            public NatPunchthroughModule Module;
        }

        public HostConnection(IClient client) : base(client)
        {
        }

        public override async Task Connect(Action onConnected, int timeout_ms = DefaultTimeout_ms)
        {
            // Tell matchmaking server to find me a host.
            byte[] introduction = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(new HostCreatingGame()));
            Client.Send(introduction, introduction.Length, MatchmakingServerEndPoint);

            // Wait for response from matchmaking server.
            CancellationTokenSource cts = new();
            cts.CancelAfter(timeout_ms);
            while (!cts.IsCancellationRequested)
            {
                UdpReceiveResult result;
                try
                {
                    result = await Client.ReceiveAsync(cts.Token);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }

                if (result.RemoteEndPoint.Equals(MatchmakingServerEndPoint))
                {
                    string strMessage = Encoding.UTF8.GetString(result.Buffer);
                    JObject json = JObject.Parse(strMessage);
                    if (json["Type"]?.ToString() == HostAck.MessageType)
                    {
                        onConnected();
                        cts.Cancel();
                        return;
                    }
                }
            }
        }

        private async Task SendMessageToAllPlayers(byte[] message)
        {
            foreach (ConnectedPlayer player in ConnectedPlayers)
            {
                await Client.SendAsync(message, player.EndPoint);
            }
        }

        private void HandleMessageFromMatchmakingServer(byte[] message)
        {
            string strMessage = Encoding.UTF8.GetString(message);
            InformOfPeer? informOfPeer;

            try
            {
                informOfPeer = JsonConvert.DeserializeObject<InformOfPeer>(strMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            if (informOfPeer == null)
                return;

            ulong id = IdGenerator.GenerateId();
            var player = new PlayerDetails(
                id: id,
                name: "player_" + id.ToString().Substring(0, 6),
                ip: informOfPeer.IpAddress,
                port: informOfPeer.Port
            );
            var connectingClient = new ConnectingClient
            {
                Player = new PlayerDetails(
                    id: id,
                    name: "player_" + id.ToString().Substring(0, 6),
                    ip: informOfPeer.IpAddress,
                    port: informOfPeer.Port
                ),
                Module = new NatPunchthroughModule(Client, player.EndPoint)
            };
            connectingClients.Add(player.EndPoint, connectingClient);
        }

        public override void HandleMessage(IPEndPoint endpoint, byte[] message)
        {
            if (endpoint == MatchmakingServerEndPoint)
            {
                HandleMessageFromMatchmakingServer(message);
            }
            else if (connectingClients.ContainsKey(endpoint))
            {
                NatPunchthroughModule module = connectingClients[endpoint].Module;
                module.HandleMessageFromPeer(message);
                if (module.ConnectionEstablished)
                {
                    ConnectedPlayers.Add(
                        new ConnectedPlayer(
                            connectingClients[endpoint].Player.Id,
                            connectingClients[endpoint].Player.EndPoint));
                    connectingClients.Remove(endpoint);
                }
            }
            else if (ConnectedWorld != null)
            {
                try
                {
                    Schema.OneofRequest request = Schema.OneofRequest.Parser.ParseFrom(message);
                    ConnectedWorld.Requests.Enqueue(request);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return;
                }
            }
        }

        public override async Task SendPendingMessages()
        {
            foreach (ConnectingClient module in connectingClients.Values)
            {
                module.Module.Update();
            }

            if (ConnectedWorld == null)
                return;

            foreach (var connectedPlayer in ConnectedPlayers)
            {
                while (UpdatePackets.ContainsKey(connectedPlayer.AssumedVersion))
                {
                    Schema.UpdatePacket? packet = UpdatePackets[connectedPlayer.AssumedVersion];
                    byte[] message = packet.ToByteArray();
                    await Client.SendAsync(message, connectedPlayer.EndPoint);
                    connectedPlayer.AssumedVersion++;
                }
            }
        }

        public void DrainUpdatesOfFrame(World world)
        {
            List<Schema.OneofUpdate> updates = new(world._updatesOfFrame.Count);
            while (world._updatesOfFrame.Count > 0)
                updates.Add(world._updatesOfFrame.Dequeue());

            List<Schema.UpdatePacket> packets =
                MessageChunker.Chunk(updates.Select(u => u.ToByteArray()).ToList());
            foreach (Schema.UpdatePacket packet in packets)
            {
                UpdatePackets.Add(CurrentVersion, packet);
                CurrentVersion++;
            }
        }
    }
}