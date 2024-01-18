using System.Text.Json.Serialization;

public class HostCreatingGame
{
    public const string MessageType = "HostCreatingGame";

    [JsonPropertyName("type")]
    public string Type = MessageType;
}