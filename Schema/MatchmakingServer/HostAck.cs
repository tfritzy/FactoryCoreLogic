using Newtonsoft.Json;

public class HostAck
{
    public const string MessageType = "HostAck";

    [JsonProperty("type")]
    public string Type = MessageType;
}