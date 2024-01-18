using Newtonsoft.Json;

public class HostCreatingGame
{
    public const string MessageType = "HostCreatingGame";

    [JsonProperty("type")]
    public string Type = MessageType;
}