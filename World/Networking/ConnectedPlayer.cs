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
        public DateTime LastSentHeartbeat { get; set; }
        public DateTime LastReceivedHeartbeat { get; private set; }
        public TimeSpan Ping { get; private set; }
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
            LastSentHeartbeat = DateTime.Now;
            LastReceivedHeartbeat = DateTime.Now;
            NumSentPackets = 0;
            NumMissedPackets = 0;
        }

        public void UpdateHeartbeat()
        {
            LastReceivedHeartbeat = DateTime.Now;
            Ping = LastReceivedHeartbeat - LastSentHeartbeat;
        }
    }
}