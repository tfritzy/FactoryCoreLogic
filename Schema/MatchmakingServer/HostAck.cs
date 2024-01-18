using System.Text.Json.Serialization;

public class HostAck
{
    public const string MessageType = "HostAck";

    [JsonPropertyName("type")]
    public string Type = MessageType;
}