using System.Net;
using Newtonsoft.Json;

namespace Core
{
    public class PlayerDetails
    {
        public ulong Id { get; }
        public string Name { get; }
        public string Ip { get; set; }
        public int Port { get; set; }

        [JsonIgnore]
        public IPEndPoint EndPoint { get; private set; }

        public PlayerDetails(ulong id, string name, string ip, int port)
        {
            Id = id;
            Name = name;
            Ip = ip;
            Port = port;
            EndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }
    }
}