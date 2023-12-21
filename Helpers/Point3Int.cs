using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core
{
    [JsonConverter(typeof(Point3IntConverter))]
    public struct Point3Int
    {
        public int x;
        public int y;
        public int z;

        public Point3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Point3Int(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public static Point3Int operator +(Point3Int p1, Point3Int p2)
        {
            return new Point3Int(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
        }

        public static Point3Int operator -(Point3Int p1, Point3Int p2)
        {
            return new Point3Int(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
        }

        public static Point3Int operator +(Point3Int p1, Point2Int p2)
        {
            return new Point3Int(p1.x + p2.x, p1.y + p2.y, p1.z);
        }

        public static Point3Int operator -(Point3Int p1, Point2Int p2)
        {
            return new Point3Int(p1.x - p2.x, p1.y - p2.y, p1.z);
        }

        public static Point3Int operator *(Point3Int p1, int p2)
        {
            return new Point3Int(p1.x * p2, p1.y * p2, p1.z * p2);
        }

        public static bool operator ==(Point3Int p1, Point3Int p2)
        {
            if (object.Equals(p1, p2))
                return true;
            if (object.Equals(p1, null) || object.Equals(p2, null))
                return false;
            return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
        }

        public static bool operator !=(Point3Int p1, Point3Int p2)
        {
            return !(p1 == p2);
        }

        public override bool Equals(object? obj)
        {
            if (object.Equals(obj, null) || !(obj is Point3Int))
                return false;

            Point3Int other = (Point3Int)obj;
            return x == other.x && y == other.y && z == other.z;
        }

        public static explicit operator Point2Int(Point3Int p)
        {
            return new Point2Int(p.x, p.y);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + x.GetHashCode();
                hash = hash * 23 + y.GetHashCode();
                hash = hash * 23 + z.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public static Point3Int Up = new(0, 0, 1);
        public static Point3Int Down = new(0, 0, -1);
        public static Point3Int Zero = new(0, 0, 0);

        public readonly Point3Int WalkNorthEast => GridHelpers.GetNeighbor(this, HexSide.NorthEast);
        public readonly Point3Int WalkEast => GridHelpers.GetNeighbor(this, HexSide.East);
        public readonly Point3Int WalkSouthEast => GridHelpers.GetNeighbor(this, HexSide.SouthEast);
        public readonly Point3Int WalkSouthWest => GridHelpers.GetNeighbor(this, HexSide.SouthWest);
        public readonly Point3Int WalkWest => GridHelpers.GetNeighbor(this, HexSide.West);
        public readonly Point3Int WalkNorthWest => GridHelpers.GetNeighbor(this, HexSide.NorthWest);
    }

    public class Point3IntConverter : JsonConverter<Point3Int>
    {
        public override Point3Int ReadJson(JsonReader reader, Type objectType, Point3Int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return new Point3Int(int.MinValue, int.MinValue, int.MinValue);

            string[] values = ((string)reader.Value).Split(',');
            int x = int.Parse(values[0]);
            int y = int.Parse(values[1]);
            int z = values.Length > 2 ? int.Parse(values[2]) : 0;
            return new Point3Int(x, y, z);
        }

        public override void WriteJson(JsonWriter writer, Point3Int value, JsonSerializer serializer)
        {
            JToken.FromObject($"{value.x},{value.y},{value.z}").WriteTo(writer);
        }
    }
}