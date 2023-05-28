using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core
{
    [JsonConverter(typeof(Point3FloatConverter))]
    public struct Point3Float
    {
        public float x;
        public float y;
        public float z;

        public Point3Float(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Point3Float(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public static Point3Float operator +(Point3Float p1, Point3Float p2)
        {
            return new Point3Float(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
        }

        public static Point3Float operator -(Point3Float p1, Point3Float p2)
        {
            return new Point3Float(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
        }

        public static Point3Float operator +(Point3Float p1, Point2Float p2)
        {
            return new Point3Float(p1.x + p2.x, p1.y + p2.y, p1.z);
        }

        public static Point3Float operator -(Point3Float p1, Point2Float p2)
        {
            return new Point3Float(p1.x - p2.x, p1.y - p2.y, p1.z);
        }

        public static Point3Float operator *(Point3Float p1, float p2)
        {
            return new Point3Float(p1.x * p2, p1.y * p2, p1.z * p2);
        }

        public static bool operator ==(Point3Float p1, Point3Float p2)
        {
            if (object.Equals(p1, p2))
                return true;
            if (object.Equals(p1, null) || object.Equals(p2, null))
                return false;
            return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
        }

        public static bool operator !=(Point3Float p1, Point3Float p2)
        {
            return !(p1 == p2);
        }

        public override bool Equals(object? obj)
        {
            if (object.Equals(obj, null) || !(obj is Point3Float))
                return false;

            Point3Float other = (Point3Float)obj;
            return x == other.x && y == other.y && z == other.z;
        }

        public static explicit operator Point2Float(Point3Float p)
        {
            return new Point2Float(p.x, p.y);
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
    }

    public class Point3FloatConverter : JsonConverter<Point3Float>
    {
        public override Point3Float ReadJson(JsonReader reader, Type objectType, Point3Float existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return new Point3Float(float.MinValue, float.MinValue, float.MinValue);

            string[] values = ((string)reader.Value).Split(',');
            float x = float.Parse(values[0]);
            float y = float.Parse(values[1]);
            float z = values.Length > 2 ? float.Parse(values[2]) : 0;
            return new Point3Float(x, y, z);
        }

        public override void WriteJson(JsonWriter writer, Point3Float value, JsonSerializer serializer)
        {
            JToken.FromObject($"{value.x},{value.y},{value.z}").WriteTo(writer);
        }
    }
}