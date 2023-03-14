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

    public static bool operator ==(Point3Int p1, Point3Int p2)
    {
        if (ReferenceEquals(p1, p2))
            return true;
        if (ReferenceEquals(p1, null) || ReferenceEquals(p2, null))
            return false;
        return p1.x == p2.x && p1.y == p2.y && p1.z == p2.z;
    }

    public static bool operator !=(Point3Int p1, Point3Int p2)
    {
        return !(p1 == p2);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(obj, null) || !(obj is Point3Int))
            return false;

        Point3Int other = (Point3Int)obj;
        return x == other.x && y == other.y && z == other.z;
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