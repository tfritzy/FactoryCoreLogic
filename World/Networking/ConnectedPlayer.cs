using System;
using System.Collections;
using System.Net;

namespace Core
{
    public class ConnectedPlayer
    {
        public Guid Id { get; private set; }
        public ulong CharacterId { get; set; }
        public IPEndPoint EndPoint { get; private set; }
        public int HighestConfirmedVersion { get; set; }
        public ulong AssumedVersion { get; set; }
        public int LastSentHeartbeat_ms { get; set; }
        public int LastReceivedHeartbeat_ms { get; private set; }
        public int Ping { get; private set; }
        public int NumSentPackets;
        public int NumMissedPackets;
        public float PacketLoss => NumMissedPackets / (float)NumSentPackets;

        public ConnectedPlayer(Guid id, IPEndPoint endPoint)
        {
            Id = id;
            CharacterId = IdGenerator.GenerateId();
            EndPoint = endPoint;
            HighestConfirmedVersion = 0;
            AssumedVersion = 0;
            LastSentHeartbeat_ms = 0;
            LastReceivedHeartbeat_ms = 0;
            NumSentPackets = 0;
            NumMissedPackets = 0;
        }

        public void UpdateHeartbeat(int currentTick)
        {
            LastReceivedHeartbeat_ms = currentTick;
            Ping = LastReceivedHeartbeat_ms - LastSentHeartbeat_ms;
        }
    }
}