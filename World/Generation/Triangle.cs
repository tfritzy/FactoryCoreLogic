using Newtonsoft.Json;

public struct Triangle
{
    [JsonProperty("t")]
    public TriangleType Type;

    [JsonProperty("s")]
    public TriangleSubType SubType;
}