public static class HexGridHelpers
{
    private static readonly Point2Int[] oddNeighborPattern = new Point2Int[]
    {
        new Point2Int(0, 1), // north
        new Point2Int(1, 1), // northeast
        new Point2Int(1, 0), // southeast
        new Point2Int(0, -1), // south
        new Point2Int(-1, 0), // southwest
        new Point2Int(-1, 1) // northwest
    };

    private static readonly Point2Int[] evenNeighborPattern = new Point2Int[]
    {
        new Point2Int(0, 1), // north
        new Point2Int(1, 0),  // northeast
        new Point2Int(1, -1), // southeast
        new Point2Int(0, -1), // south
        new Point2Int(-1,-1), // southwest
        new Point2Int(-1, 0) // northwest
    };

    public static Point2Int GetNeighbor(Point2Int pos, HexSide direction)
    {
        Point2Int position;

        if (pos.x % 2 == 0)
        {
            position = pos + evenNeighborPattern[(int)direction];
        }
        else
        {
            position = pos + oddNeighborPattern[(int)direction];
        }

        return position;
    }

    public static Point3Int GetNeighbor(Point3Int pos, HexSide direction)
    {
        Point3Int position = pos;

        if (direction == HexSide.Up)
        {
            position.z += 1;
            return position;
        }

        if (direction == HexSide.Down)
        {
            position.z -= 1;
            return position;
        }

        if (pos.x % 2 == 0)
        {
            position = pos + evenNeighborPattern[(int)direction];
        }
        else
        {
            position = pos + oddNeighborPattern[(int)direction];
        }

        return position;
    }

    public static bool IsInBounds(Point3Int point, Hex[,,] grid)
    {
        if (point.x < 0 || point.x >= grid.GetLength(0))
        {
            return false;
        }

        if (point.y < 0 || point.y >= grid.GetLength(1))
        {
            return false;
        }

        if (point.z < 0 || point.z >= grid.GetLength(2))
        {
            return false;
        }

        return true;
    }
}