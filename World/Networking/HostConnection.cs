using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Core
{
    public class HostConnection : Connection
    {
        public const string HostCreatingGame = "HostCreatingGame";
        public const string HostAck = "AckHost";
        public List<PlayerDetails> ConnectedPlayers = new();
        private Dictionary<IPEndPoint, ConnectingClient> connectingClients = new();

        struct ConnectingClient
        {
            public PlayerDetails Player;
            public NatPunchthroughModule Module;
        }

        public HostConnection(Context context, IClient client) : base(context, client)
        {
        }

        public override async Task Connect(int timeout_ms = DefaultTimeout_ms)
        {
            if (State != ConnectionState.Disconnected)
                return;

            // Tell matchmaking server I am a host
            State = ConnectionState.Connecting;

            // Tell matchmaking server to find me a host.
            byte[] introduction = Encoding.UTF8.GetBytes(HostCreatingGame);
            client.Send(introduction, introduction.Length, MatchmakingServerEndPoint);

            // Wait for response from matchmaking server.
            CancellationTokenSource cts = new();
            cts.CancelAfter(timeout_ms);
            while (!cts.IsCancellationRequested)
            {
                UdpReceiveResult result;
                try
                {
                    result = await client.ReceiveAsync(cts.Token);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }

                if (result.RemoteEndPoint.Equals(MatchmakingServerEndPoint))
                {
                    string strMessage = Encoding.UTF8.GetString(result.Buffer);
                    if (strMessage == HostAck)
                    {
                        State = ConnectionState.Connected;
                        cts.Cancel();
                        return;
                    }
                }
            }

            State = ConnectionState.Disconnected;
        }

        private void SendMessageToAllPlayers(byte[] message)
        {
            foreach (PlayerDetails player in ConnectedPlayers)
            {
                client.Send(message, message.Length, player.EndPoint);
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
                Module = new NatPunchthroughModule(client, playerDetails.EndPoint)
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
            else
            {
                try
                {
                    Schema.OneofRequest request = Schema.OneofRequest.Parser.ParseFrom(message);
                    context.World.Requests.Enqueue(request);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return;
                }
            }
        }

        public override void SendPendingMessages()
        {
            while (context.World.Updates.Count > 0)
            {
                Schema.OneofUpdate update = context.World.Updates.Dequeue();
                byte[] message = update.ToByteArray();
                SendMessageToAllPlayers(message);
            }

            foreach (ConnectingClient module in connectingClients.Values)
            {
                module.Module.Update();
            }
        }
    }
}