using System.Net;

namespace Core
{
    public class PlayerDetails
    {
        public ulong Id { get; }
        public string Name { get; }
        public IPEndPoint EndPoint { get; set; }

        public PlayerDetails(ulong id, string name, IPEndPoint endPoint)
        {
            Id = id;
            Name = name;
            EndPoint = endPoint;
        }
    }
}