using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private List<Packet?> receivedPackets = new();
        private Action? onConnected;
        private HashSet<ulong> missedPacketIds = new();
        public ulong NextNeededPacket { get; private set; } = 0;
        public ulong HighestHandledPacket { get; private set; } = 0;
        public int NumPacketsReceived;
        public Action<World>? OnSetWorld;
        protected List<Schema.OneofRequest> requestsOfFrame = new();

        public ClientConnection(IClient client, Action? onConnected = null) : base(client)
        {
            this.onConnected = onConnected;
        }

        public override async Task Connect(int timeout_ms = DefaultTimeout_ms)
        {
            // Tell matchmaking server to find me a host.
            byte[] introduction = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(new ClientLookingForHost(PlayerId)));
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

        public override async Task HandleMessage(IPEndPoint endpoint, byte[] message)
        {
            NumPacketsReceived++;

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
                Packet packet = Packet.Parser.ParseFrom(message);
                if (packet.Type == PacketType.Heartbeat)
                {
                    var response = new HeartbeatResponse();
                    response.MissedPacketIds.AddRange(missedPacketIds);
                    await Client.SendAsync(
                        new OneofRequest()
                        {
                            Heartbeat = response
                        }.ToByteArray(),
                        HostEndPoint
                    );
                }
                else
                {
                    if (packet.Id == NextNeededPacket)
                    {
                        receivedPackets.Add(packet);
                        NextNeededPacket = packet.Id + 1;
                    }
                    else if (packet.Id > NextNeededPacket)
                    {
                        for (ulong i = NextNeededPacket; i < packet.Id; i++)
                        {
                            receivedPackets.Add(null);
                            missedPacketIds.Add(i);
                        }

                        receivedPackets.Add(packet);
                        NextNeededPacket = packet.Id + 1;
                    }
                    else if (missedPacketIds.Contains(packet.Id))
                    {
                        missedPacketIds.Remove(packet.Id);
                        receivedPackets[(int)(packet.Id - HighestHandledPacket)] = packet;
                    }

                    if (ConnectedWorld == null)
                    {
                        int previousLength = receivedPackets.Count;
                        Schema.OneofUpdate? maybeWorldState = MessageChunker.ExtractFullUpdate(ref receivedPackets);
                        if (maybeWorldState?.WorldState != null)
                        {
                            World world = new World(maybeWorldState.WorldState.World, new Context());
                            SetWorld(world);
                            int numPacketsRemoved = previousLength - receivedPackets.Count;
                            HighestHandledPacket += (ulong)numPacketsRemoved;
                            OnSetWorld?.Invoke(world);
                        }
                    }

                    if (ConnectedWorld != null)
                    {
                        int previousLength = receivedPackets.Count;
                        while (MessageChunker.ExtractFullUpdate(ref receivedPackets) is Schema.OneofUpdate fullUpdate)
                        {
                            ConnectedWorld.HandleUpdate(fullUpdate);
                            int numPacketsRemoved = previousLength - receivedPackets.Count;
                            HighestHandledPacket += (ulong)numPacketsRemoved;
                        }
                    }
                }
            }
        }

        public override async Task SendPendingMessages()
        {
            punchthrough?.Update();

            if (ConnectedWorld != null)
            {
                foreach (var request in requestsOfFrame)
                {
                    // TODO: Ensure delivery.
                    await SendMessage(request);
                }
                requestsOfFrame.Clear();
            }
        }

        public override async Task HandleRequest(OneofRequest request)
        {
            if (request.VelocityChange != null)
            {
                // Only keep the most recent move request for a given player.
                int existingIndex =
                    requestsOfFrame.FindIndex(r => r.VelocityChange?.PlayerId == request.VelocityChange.PlayerId);
                if (existingIndex != -1)
                {
                    requestsOfFrame[existingIndex] = request;
                }
                else
                {
                    requestsOfFrame.Add(request);
                }
            }
            else
            {
                requestsOfFrame.Add(request);
            }
        }
    }
}