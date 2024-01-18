using Newtonsoft.Json;

public class ClientAck
{
    public const string MessageType = "ClientAck";

    [JsonProperty("type")]
    public string Type = MessageType;
}