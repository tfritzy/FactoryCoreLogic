using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public static class NatPunchthrough
    {
        public const int Timeout_ms = 15_000;
        public const float InitialTimeBetweenSends_s = .1f;
        public const string IntroductionMessage = "P2PIntroduction";
        public const string AckMessage = "AckIntroduction";
        public const string HandshakeComplete = "HandshakeComplete";

        public static async Task<bool> Punchthrough(IClient client, IPEndPoint peerEndpoint, int timeout_ms = Timeout_ms)
        {
            CancellationTokenSource cts = new();
            cts.CancelAfter(timeout_ms);
            float timeBetweenSends_s = InitialTimeBetweenSends_s;
            Task sendHelloTask = Task.Run(async () =>
            {
                byte[] introduction = Encoding.UTF8.GetBytes(IntroductionMessage);
                while (!cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        client.Send(introduction, introduction.Length, peerEndpoint);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(timeBetweenSends_s), cts.Token);
                    timeBetweenSends_s *= 1.5f;
                }
            }, cts.Token);

            bool connectionEstablished = false;
            Task listenForAckTask = Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    UdpReceiveResult result = default;
                    try
                    {
                        result = await client.ReceiveAsync(cts.Token);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    if (result.RemoteEndPoint.Equals(peerEndpoint))
                    {
                        string strMessage = Encoding.UTF8.GetString(result.Buffer);
                        if (strMessage == IntroductionMessage)
                        {
                            byte[] ack = Encoding.UTF8.GetBytes(AckMessage);
                            client.Send(ack, ack.Length, peerEndpoint);

                            if (connectionEstablished)
                            {
                                // If they're still unsure things are good, send another handshake complete
                                byte[] handshakeComplete = Encoding.UTF8.GetBytes(HandshakeComplete);
                                client.Send(handshakeComplete, handshakeComplete.Length, peerEndpoint);
                            }
                        }
                        else if (strMessage == AckMessage)
                        {
                            connectionEstablished = true;

                            byte[] handshakeComplete = Encoding.UTF8.GetBytes(HandshakeComplete);
                            client.Send(handshakeComplete, handshakeComplete.Length, peerEndpoint);
                        }
                        else if (strMessage == HandshakeComplete)
                        {
                            // We know both parties are happy, so we can stop the handshake.
                            connectionEstablished = true;
                            cts.Cancel();
                            break;
                        }
                    }
                }
            });

            await Task.WhenAny(sendHelloTask, listenForAckTask);

            return connectionEstablished;
        }
    }
}