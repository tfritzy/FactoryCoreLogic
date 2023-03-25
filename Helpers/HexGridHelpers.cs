using System.Collections.Generic;

namespace FactoryCore
{
    public static class GridHelpers
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

        private static readonly Dictionary<Point2Int, HexSide> oddNeighborPatternMap = new Dictionary<Point2Int, HexSide>()
    {
        { new Point2Int(0, 1), HexSide.North },
        { new Point2Int(1, 1), HexSide.NorthEast },
        { new Point2Int(1, 0), HexSide.SouthEast },
        { new Point2Int(0, -1), HexSide.South },
        { new Point2Int(-1, 0), HexSide.SouthWest },
        { new Point2Int(-1, 1), HexSide.NorthWest }
    };

        private static readonly Dictionary<Point2Int, HexSide> evenNeighborPatternMap = new Dictionary<Point2Int, HexSide>()
    {
        { new Point2Int(0, 1), HexSide.North },
        { new Point2Int(1, 0), HexSide.NorthEast },
        { new Point2Int(1, -1), HexSide.SouthEast },
        { new Point2Int(0, -1), HexSide.South },
        { new Point2Int(-1,-1), HexSide.SouthWest },
        { new Point2Int(-1, 0), HexSide.NorthWest }
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

        public static bool IsInBounds(Point3Int pos, Hex?[,,] grid)
        {
            return IsInBounds(pos.x, pos.y, pos.z, grid);
        }

        public static bool IsInBounds(int x, int y, int z, Hex?[,,] grid)
        {
            if (x < 0 || x >= grid.GetLength(0))
            {
                return false;
            }

            if (y < 0 || y >= grid.GetLength(1))
            {
                return false;
            }

            if (z < 0 || z >= grid.GetLength(2))
            {
                return false;
            }

            return true;
        }

        public static HexSide? GetNeighborSide(Point2Int pos, Point2Int neighborPos)
        {
            Point2Int direction = neighborPos - pos;

            if (pos.x % 2 == 0)
            {
                if (evenNeighborPatternMap.ContainsKey(direction))
                {
                    return evenNeighborPatternMap[direction];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (oddNeighborPatternMap.ContainsKey(direction))
                {
                    return oddNeighborPatternMap[direction];
                }
                else
                {
                    return null;
                }
            }
        }

        public static HexSide OppositeSide(HexSide side)
        {
            return (HexSide)(((int)side + 3) % 6);
        }
    }
}