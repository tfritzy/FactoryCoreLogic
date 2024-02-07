using System;
using System.Net;
using Newtonsoft.Json;

namespace Core
{
    public class PlayerDetails
    {
        public Guid Id { get; }
        public string Name { get; }
        public string IpAddress { get; set; }
        public int Port { get; set; }

        [JsonIgnore]
        public IPEndPoint EndPoint { get; private set; }

        public PlayerDetails(Guid id, string name, string ip, int port)
        {
            Id = id;
            Name = name;
            IpAddress = ip;
            Port = port;
            EndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }
    }
}