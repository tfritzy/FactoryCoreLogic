using System;
using System.ComponentModel;
using System.Globalization;

namespace Core
{
    [TypeConverter(typeof(Point2FloatConverter))]
    public struct Point2Float
    {
        public float x;
        public float y;

        public Point2Float(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point2Float operator +(Point2Float p1, Point2Float p2)
        {
            return new Point2Float(p1.x + p2.x, p1.y + p2.y);
        }

        public static Point2Float operator -(Point2Float p1, Point2Float p2)
        {
            return new Point2Float(p1.x - p2.x, p1.y - p2.y);
        }

        public static Point2Float operator +(Point2Float p1, Point3Float p2)
        {
            return new Point2Float(p1.x + p2.x, p1.y + p2.y);
        }

        public static Point2Float operator -(Point2Float p1, Point3Float p2)
        {
            return new Point2Float(p1.x - p2.x, p1.y - p2.y);
        }

        public static bool operator ==(Point2Float p1, Point2Float p2)
        {
            if (object.Equals(p1, p2))
                return true;
            if (object.Equals(p1, null) || object.Equals(p2, null))
                return false;
            return p1.x == p2.x && p1.y == p2.y;
        }

        public static bool operator !=(Point2Float p1, Point2Float p2)
        {
            return !(p1 == p2);
        }

        public override bool Equals(object? obj)
        {
            if (object.Equals(obj, null) || !(obj is Point2Float))
                return false;

            Point2Float other = (Point2Float)obj;
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x.GetHashCode() * 397) ^ y.GetHashCode();
            }
        }


        public override string ToString()
        {
            return $"{x},{y}";
        }
    }

    public class Point2FloatConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string stringValue)
            {
                var parts = stringValue.Split(',');
                if (parts.Length == 2 && float.TryParse(parts[0], out float x) && float.TryParse(parts[1], out float y))
                {
                    return new Point2Float(x, y);
                }
            }

            throw new NotSupportedException($"Cannot convert \"{value}\" to {typeof(Point2Float)}.");
        }
    }
}