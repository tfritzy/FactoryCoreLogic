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
        private List<Packet> receivedPackets = new();
        private Action? onConnected;

        public ClientConnection(IClient client, Action? onConnected = null) : base(client)
        {
            this.onConnected = onConnected;
        }

        public override async Task Connect(int timeout_ms = DefaultTimeout_ms)
        {
            // Tell matchmaking server to find me a host.
            byte[] introduction = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(new ClientLookingForHost()));
            Client.Send(introduction, introduction.Length, MatchmakingServerEndPoint);
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
            JObject json = JObject.Parse(strMessage);
            InformOfPeer? informOfPeer;

            if (json["Type"]?.ToString() == ClientAck.MessageType)
            {
                onConnected?.Invoke();
                return;
            }
            else if (json["Type"]?.ToString() == InformOfPeer.MessageType)
            {
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
                punchthrough = new NatPunchthroughModule(Client, HostEndPoint, this.onConnected);
            }
        }

        public override void HandleMessage(IPEndPoint endpoint, byte[] message)
        {
            if (endpoint.Equals(MatchmakingServerEndPoint))
            {
                HandleMessageFromMatchmakingServer(message);
            }
            else if (punchthrough?.IsMessageForNatPunchthrough(message) ?? false)
            {
                punchthrough.HandleMessageFromPeer(message);
            }
            else if (endpoint.Equals(HostEndPoint))
            {
                Packet update = Packet.Parser.ParseFrom(message);
                receivedPackets.Add(update);
                if (ConnectedWorld != null)
                {
                    while (MessageChunker.ExtractFullUpdate(ref receivedPackets) is Schema.OneofUpdate fullUpdate)
                    {
                        ConnectedWorld.HandleUpdate(fullUpdate);
                    }
                }
                else
                {
                    Schema.OneofUpdate? maybeWorldState = MessageChunker.ExtractFullUpdate(ref receivedPackets);
                    if (maybeWorldState?.WorldState != null)
                    {
                        World world = new World(maybeWorldState.WorldState.World, new Context());
                        SetWorld(world);
                    }
                }
            }
        }

        public override async Task SendPendingMessages()
        {
            if (ConnectedWorld != null)
            {
                while (ConnectedWorld.Requests.Count > 0)
                {
                    await SendMessage(ConnectedWorld.Requests.Dequeue());
                }
            }

            punchthrough?.Update();
        }
    }
}