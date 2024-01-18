using System.Text.Json.Serialization;

public class ClientAck
{
    public const string MessageType = "ClientAck";

    [JsonPropertyName("type")]
    public string Type = MessageType;
}