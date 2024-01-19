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

namespace Core
{
    public class HostConnection : Connection
    {
        public List<PlayerDetails> ConnectedPlayers = new();
        private Dictionary<IPEndPoint, ConnectingClient> connectingClients = new();

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
            foreach (PlayerDetails player in ConnectedPlayers)
            {
                await Client.SendAsync(message, player.EndPoint);
            }
        }

        private void HandleMessageFromMatchmakingServer(byte[] message)
        {
            string strMessage = Encoding.UTF8.GetString(message);
            PlayerDetails? playerDetails;

            try
            {
                playerDetails = JsonConvert.DeserializeObject<PlayerDetails>(strMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            if (playerDetails == null)
                return;

            var connectingClient = new ConnectingClient
            {
                Player = playerDetails,
                Module = new NatPunchthroughModule(Client, playerDetails.EndPoint)
            };
            connectingClients.Add(playerDetails.EndPoint, connectingClient);
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
                    ConnectedPlayers.Add(connectingClients[endpoint].Player);
                    connectingClients.Remove(endpoint);
                }
            }
            else if (InterestedWorlds.Count > 0)
            {
                try
                {
                    Schema.OneofRequest request = Schema.OneofRequest.Parser.ParseFrom(message);
                    InterestedWorlds[0].Requests.Enqueue(request);
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
            foreach (World world in InterestedWorlds)
            {
                while (world.Updates.Count > 0)
                {
                    Schema.OneofUpdate update = world.Updates.Dequeue();
                    byte[] message = update.ToByteArray();
                    await SendMessageToAllPlayers(message);
                }
            }

            foreach (ConnectingClient module in connectingClients.Values)
            {
                module.Module.Update();
            }
        }
    }
}