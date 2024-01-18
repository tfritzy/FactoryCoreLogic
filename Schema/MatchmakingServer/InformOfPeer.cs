public class InformOfPeer
{
    public string IpAddress;
    public int Port;

    public const string MessageType = "InformOfPeer";
    public string Type = MessageType;

    public InformOfPeer(string ipAddress, int port)
    {
        IpAddress = ipAddress;
        Port = port;
    }
}