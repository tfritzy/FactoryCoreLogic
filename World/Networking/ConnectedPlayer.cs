using System;
using System.Net;

namespace Core
{
    public class ConnectedPlayer
    {
        public ulong Id { get; private set; }
        public IPEndPoint EndPoint { get; private set; }
        public int HighestConfirmedVersion { get; set; }
        public ulong AssumedVersion { get; set; }
        public DateTime LastSentHeartbeat { get; set; }
        public DateTime LastReceivedHeartbeat { get; set; }
        public TimeSpan Ping => LastReceivedHeartbeat - LastSentHeartbeat;
        public int NumSentPackets;
        public int NumMissedPackets;
        public float PacketLoss => NumMissedPackets / (float)NumSentPackets;

        public ConnectedPlayer(ulong id, IPEndPoint endPoint)
        {
            Id = id;
            EndPoint = endPoint;
            HighestConfirmedVersion = 0;
            AssumedVersion = 0;
            LastSentHeartbeat = DateTime.Now;
            LastReceivedHeartbeat = DateTime.Now;
            NumSentPackets = 0;
            NumMissedPackets = 0;
        }
    }
}