using System.Text;
using Newtonsoft.Json;

public class HostAck
{
    public const string MessageType = "HostAck";
    public string Type = MessageType;

    public byte[] ToBytes()
    {
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
    }
}