using System;
using Newtonsoft.Json;

public class Triangle
{
    [JsonProperty("t")]
    public TriangleType Type;

    [JsonProperty("s")]
    public TriangleSubType SubType;

    public Triangle(TriangleType type, TriangleSubType subType)
    {
        Type = type;
        SubType = subType;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Triangle triangle)
        {
            return triangle.Type == Type && triangle.SubType == SubType;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, SubType);
    }

    public static bool operator !=(Triangle? p1, Triangle? p2)
    {
        return !(p1 == p2);
    }

    public static bool operator ==(Triangle? p1, Triangle? p2)
    {
        if (object.Equals(p1, p2))
            return true;
        if (object.Equals(p1, null) || object.Equals(p2, null))
            return false;
        return p1.Type == p2.Type && p1.SubType == p2.SubType;
    }
}