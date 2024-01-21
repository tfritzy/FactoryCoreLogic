using System.Text;
using Newtonsoft.Json;

public class ClientAck
{
    public const string MessageType = "ClientAck";

    [JsonProperty("type")]
    public string Type = MessageType;

    public byte[] ToBytes()
    {
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
    }
}