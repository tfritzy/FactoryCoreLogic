using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;

public class InformOfPeer
{
    public string IpAddress;
    public int Port;
    public Guid Id;

    public const string MessageType = "InformOfPeer";
    public string Type = MessageType;

    [JsonIgnore]
    public IPEndPoint EndPoint => new(IPAddress.Parse(IpAddress), Port);

    public InformOfPeer(string ipAddress, int port, Guid id)
    {
        IpAddress = ipAddress;
        Port = port;
        Id = id;
    }

    public byte[] ToBytes()
    {
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
    }
}