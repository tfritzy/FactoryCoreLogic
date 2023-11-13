using Newtonsoft.Json;

public class Triangle
{
    [JsonProperty("t")]
    public TriangleType Type;

    [JsonProperty("s")]
    public TriangleSubType SubType;
}