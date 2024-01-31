using System.Net;

namespace Core
{
    public class ConnectedPlayer
    {
        public ulong Id { get; private set; }
        public IPEndPoint EndPoint { get; private set; }
        public int HighestConfirmedVersion { get; set; }
        public ulong AssumedVersion { get; set; }

        public ConnectedPlayer(ulong id, IPEndPoint endPoint)
        {
            Id = id;
            EndPoint = endPoint;
            HighestConfirmedVersion = 0;
            AssumedVersion = 0;
        }
    }
}