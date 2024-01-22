using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Schema;

namespace Core
{
    public class ClientConnection : Connection
    {
        public IPEndPoint? HostEndPoint { get; private set; }
        private NatPunchthroughModule? punchthrough;
        private List<UpdatePacket> receivedPackets = new();

        public ClientConnection(IClient client) : base(client) { }

        public override async Task Connect(Action onConnected, int timeout_ms = DefaultTimeout_ms)
        {
            // Tell matchmaking server to find me a host.
            byte[] introduction = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(new ClientLookingForHost()));
            Client.Send(introduction, introduction.Length, MatchmakingServerEndPoint);

            // Wait for response from matchmaking server.
            CancellationTokenSource cts = new();
            cts.CancelAfter(timeout_ms);
            while (HostEndPoint == null && !cts.IsCancellationRequested)
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
                        JObject json = JObject.Parse(strMessage);
                        if (json["Type"]?.ToString() == ClientAck.MessageType)
                        {
                            cts.Cancel();
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        public async Task SendMessage(Schema.OneofRequest request)
        {
            if (HostEndPoint == null)
            {
                throw new Exception("Cannot send message to host, hostEndPoint is null.");
            }

            byte[] message = request.ToByteArray();
            await Client.SendAsync(message, HostEndPoint);
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
            {
                return;
            }

            HostEndPoint = new IPEndPoint(IPAddress.Parse(informOfPeer.IpAddress), informOfPeer.Port);
            punchthrough = new NatPunchthroughModule(Client, HostEndPoint, () => { });
        }

        public override void HandleMessage(IPEndPoint endpoint, byte[] message)
        {
            if (endpoint == MatchmakingServerEndPoint)
            {
                HandleMessageFromMatchmakingServer(message);
            }
            else if (punchthrough?.IsMessageForNatPunchthrough(message) ?? false)
            {
                punchthrough.HandleMessageFromPeer(message);
            }
            else if (ConnectedWorld != null)
            {
                UpdatePacket update = UpdatePacket.Parser.ParseFrom(message);
                receivedPackets.Add(update);
                while (MessageChunker.ExtractFullUpdate(ref receivedPackets) is byte[] fullUpdate)
                {
                    ConnectedWorld.HandleUpdate(OneofUpdate.Parser.ParseFrom(fullUpdate));
                }
            }
        }

        public override async Task SendPendingMessages()
        {
            if (ConnectedWorld == null)
            {
                return;
            }

            while (ConnectedWorld.Requests.Count > 0)
            {
                await SendMessage(ConnectedWorld.Requests.Dequeue());
            }

            punchthrough?.Update();
        }
    }
}