using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Core
{
    public class HostConnection : Connection
    {
        public List<ConnectedPlayer> ConnectedPlayers = new();
        private Dictionary<IPEndPoint, ConnectingClient> connectingClients = new();
        private Dictionary<ulong, Schema.Packet> Packets = new();
        private ulong CurrentVersion = 0;
        private Action? onConnected;
        private TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(.1f);

        struct ConnectingClient
        {
            public PlayerDetails Player;
            public NatPunchthroughModule Module;
        }

        public HostConnection(IClient client, Action? onConnected = null) : base(client)
        {
            this.onConnected = onConnected;
        }

        public override async Task Connect(int timeout_ms = DefaultTimeout_ms)
        {
            // Tell matchmaking server to find me a host.
            byte[] introduction = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(new HostCreatingGame()));
            await Client.SendAsync(introduction, MatchmakingServerEndPoint);
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
            JObject json = JObject.Parse(strMessage);

            if (json["Type"]?.ToString() == HostAck.MessageType)
            {
                onConnected?.Invoke();
                return;
            }
            else if (json["Type"]?.ToString() == InformOfPeer.MessageType)
            {
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
        }

        public override async Task HandleMessage(IPEndPoint endpoint, byte[] message)
        {
            if (endpoint.Equals(MatchmakingServerEndPoint))
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

                    if (request.Heartbeat != null)
                    {
                        var player = ConnectedPlayers.Find(player => player.EndPoint.Equals(endpoint));
                        if (player != null)
                        {
                            player.UpdateHeartbeat();
                            var neededIds = request.Heartbeat.MissedPacketIds;
                            for (int i = 0; i < neededIds.Count; i++)
                            {
                                player.NumMissedPackets++;
                                if (Packets.ContainsKey(neededIds[i]))
                                {
                                    await Client.SendAsync(Packets[neededIds[i]].ToByteArray(), endpoint);
                                }
                            }
                        }
                    }
                    else
                    {
                        ConnectedWorld.Requests.Enqueue(request);
                    }
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

            DrainUpdatesOfFrame();

            foreach (var connectedPlayer in ConnectedPlayers)
            {
                while (Packets.ContainsKey(connectedPlayer.AssumedVersion))
                {
                    Schema.Packet? packet = Packets[connectedPlayer.AssumedVersion];
                    byte[] message = packet.ToByteArray();
                    connectedPlayer.NumSentPackets++;
                    await Client.SendAsync(message, connectedPlayer.EndPoint);
                    connectedPlayer.AssumedVersion++;
                }

                if (connectedPlayer.LastSentHeartbeat + HeartbeatInterval < DateTime.Now)
                {
                    await Client.SendAsync(
                        new Schema.Packet { Type = Schema.PacketType.Heartbeat }
                            .ToByteArray(),
                        connectedPlayer.EndPoint);
                    connectedPlayer.LastSentHeartbeat = DateTime.Now;
                }
            }
        }

        public void DrainUpdatesOfFrame()
        {
            if (ConnectedWorld == null)
                return;

            List<Schema.OneofUpdate> updates = new(ConnectedWorld._updatesOfFrame.Count);
            while (ConnectedWorld._updatesOfFrame.Count > 0)
                updates.Add(ConnectedWorld._updatesOfFrame.Dequeue());

            List<Schema.Packet> packets = MessageChunker.Chunk(updates, CurrentVersion);
            foreach (Schema.Packet packet in packets)
            {
                Packets.Add(CurrentVersion, packet);
                CurrentVersion++;
            }
        }

        public override void SetWorld(World world)
        {
            base.SetWorld(world);
            var worldUpdate = new Schema.OneofUpdate
            {
                WorldState = new Schema.WorldState
                {
                    World = world.ToSchema()
                }
            };
            CurrentVersion = 0;
            var packets = MessageChunker.Chunk(
                new List<Schema.OneofUpdate> { worldUpdate },
                CurrentVersion);

            foreach (var packet in packets)
            {
                Packets.Add(CurrentVersion, packet);
                CurrentVersion++;
            }
        }
    }
}