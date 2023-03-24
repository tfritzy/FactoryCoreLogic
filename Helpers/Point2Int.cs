using System;
using System.ComponentModel;
using System.Globalization;

namespace FactoryCore
{
    [TypeConverter(typeof(Point2IntConverter))]
    public struct Point2Int
    {
        public int x;
        public int y;

        public Point2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point2Int operator +(Point2Int p1, Point2Int p2)
        {
            return new Point2Int(p1.x + p2.x, p1.y + p2.y);
        }

        public static Point2Int operator -(Point2Int p1, Point2Int p2)
        {
            return new Point2Int(p1.x - p2.x, p1.y - p2.y);
        }

        public static Point2Int operator +(Point2Int p1, Point3Int p2)
        {
            return new Point2Int(p1.x + p2.x, p1.y + p2.y);
        }

        public static Point2Int operator -(Point2Int p1, Point3Int p2)
        {
            return new Point2Int(p1.x - p2.x, p1.y - p2.y);
        }

        public static bool operator ==(Point2Int p1, Point2Int p2)
        {
            if (object.Equals(p1, p2))
                return true;
            if (object.Equals(p1, null) || object.Equals(p2, null))
                return false;
            return p1.x == p2.x && p1.y == p2.y;
        }

        public static bool operator !=(Point2Int p1, Point2Int p2)
        {
            return !(p1 == p2);
        }

        public override bool Equals(object? obj)
        {
            if (object.Equals(obj, null) || !(obj is Point2Int))
                return false;

            Point2Int other = (Point2Int)obj;
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + x.GetHashCode();
                hash = hash * 23 + y.GetHashCode();
                return hash;
            }
        }


        public override string ToString()
        {
            return $"{x}|{y}";
        }
    }

    public class Point2IntConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string stringValue)
            {
                var parts = stringValue.Split('|');
                if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                {
                    return new Point2Int(x, y);
                }
            }

            throw new NotSupportedException($"Cannot convert \"{value}\" to {typeof(Point2Int)}.");
        }
    }
}