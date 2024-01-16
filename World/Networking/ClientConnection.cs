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

        public ClientConnection(Context context, IClient client)
            : base(context, client)
        {
        }

        public override async Task Connect(int timeout_ms = DefaultTimeout_ms)
        {
            State = ConnectionState.Connecting;

            // Tell matchmaking server to find me a host.
            byte[] introduction = Encoding.UTF8.GetBytes(ClientLookingForHost);
            client.Send(introduction, introduction.Length, MatchmakingServerEndPoint);

            // Wait for response from matchmaking server.
            CancellationTokenSource cts = new();
            cts.CancelAfter(timeout_ms);
            while (hostEndPoint == null && !cts.IsCancellationRequested)
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
                State = ConnectionState.Disconnected;
                return;
            }

            punchthrough = new NatPunchthroughModule(client, hostEndPoint);
        }

        public void SendMessage(Schema.OneofRequest request)
        {
            byte[] message = request.ToByteArray();
            client.Send(message, message.Length, hostEndPoint);
        }

        public override void HandleMessage(IPEndPoint endpoint, byte[] message)
        {
            if (punchthrough?.IsMessageForNatPunchthrough(message) ?? false)
            {
                punchthrough.HandleMessageFromPeer(message);
                if (punchthrough.ConnectionEstablished)
                {
                    State = ConnectionState.Connected;
                }
            }
            else
            {
                Schema.OneofUpdate update = Schema.OneofUpdate.Parser.ParseFrom(message);
                context.World.Updates.Enqueue(update);
            }
        }

        public override void SendPendingMessages()
        {
            while (context.World.Requests.Count > 0)
            {
                SendMessage(context.World.Requests.Dequeue());
            }

            punchthrough?.Update();
        }
    }
}