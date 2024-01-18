using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Core
{
    public class ClientConnection : Connection
    {
        public const string ClientLookingForHost = "ClientLookingForHost";
        private IPEndPoint? hostEndPoint;
        private NatPunchthroughModule? punchthrough;

        public ClientConnection(IClient client) : base(client) { }

        public override async Task Connect(int timeout_ms = DefaultTimeout_ms)
        {
            // Tell matchmaking server to find me a host.
            byte[] introduction = Encoding.UTF8.GetBytes(ClientLookingForHost);
            Client.Send(introduction, introduction.Length, MatchmakingServerEndPoint);

            // Wait for response from matchmaking server.
            CancellationTokenSource cts = new();
            cts.CancelAfter(timeout_ms);
            while (hostEndPoint == null && !cts.IsCancellationRequested)
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
                    try
                    {
                        string ip = strMessage.Split(':')[0];
                        int port = int.Parse(strMessage.Split(':')[1]);
                        hostEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    cts.Cancel();
                    break;
                }
            }

            if (hostEndPoint == null)
            {
                return;
            }

            punchthrough = new NatPunchthroughModule(Client, hostEndPoint);
        }

        public void SendMessage(Schema.OneofRequest request)
        {
            byte[] message = request.ToByteArray();
            Client.Send(message, message.Length, hostEndPoint);
        }

        public override void HandleMessage(IPEndPoint endpoint, byte[] message)
        {
            if (punchthrough?.IsMessageForNatPunchthrough(message) ?? false)
            {
                punchthrough.HandleMessageFromPeer(message);
            }
            else if (InterestedWorlds.Count > 0)
            {
                Schema.OneofUpdate update = Schema.OneofUpdate.Parser.ParseFrom(message);
                InterestedWorlds[0].Updates.Enqueue(update);
            }
        }

        public override void SendPendingMessages()
        {
            foreach (World world in InterestedWorlds)
            {
                while (world.Requests.Count > 0)
                {
                    SendMessage(world.Requests.Dequeue());
                }
            }

            punchthrough?.Update();
        }
    }
}