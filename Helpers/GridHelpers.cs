using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
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

        public static Point2Int GetNeighbor(int x, int y, HexSide direction)
        {
            return GetNeighbor(new Point2Int(x, y), direction);
        }

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

        public static Point2Int CubeToOffset(CubeCoord cube)
        {
            return CubeToOffset(cube.q, cube.r, cube.s);
        }

        public static Point2Int CubeToOffset(int q, int r, int s)
        {
            var col = q;
            var row = r + (q - (q & 1)) / 2;
            return new Point2Int(col, row);
        }

        public static CubeCoord OffsetToCube(Point2Int point)
        {
            int q = point.x;
            int r = point.y - (point.x - (point.x & 1)) / 2;
            int s = -q - r;

            return new CubeCoord(q, r, s);
        }

        public static CubeCoord OffsetToCube(Point2Float point)
        {
            float q = point.x;
            float r = point.y - (point.x - ((int)point.x & 1)) / 2f;
            float s = -q - r;

            return new CubeCoord((int)q, (int)r, (int)s);
        }

        /*
            Stolen from: https://www.redblobgames.com/grids/hexagons/more-pixel-to-hex.html#boristhebrave
        */
        private static float[] pick_tri(float x, float y)
        {
            return new float[] {
                MathF.Ceiling(( 1 * x - MathF.Sqrt(3) / 3 * y) / Constants.HEX_RADIUS),
                MathF.Floor((    MathF.Sqrt(3) * 2 / 3 * y) / Constants.HEX_RADIUS) + 1,
                MathF.Ceiling((-1 * x - MathF.Sqrt(3) / 3 * y) / Constants.HEX_RADIUS)
            };
        }

        private static CubeCoord tri_to_hex(float x, float y, float z)
        {
            return new CubeCoord(
                (int)MathF.Round((x - z) / 3),
                (int)MathF.Round((y - x) / 3),
                (int)MathF.Round((z - y) / 3) // not needed for axial
            );
        }

        public static Point3Int CartesianToGrid(Point3Float point)
        {
            var abc = pick_tri(point.x, point.y); // swap for pointy/flat
            var cube = tri_to_hex(abc[0], abc[1], abc[2]);
            Point3Int result = (Point3Int)CubeToOffset(cube);
            result.z = (int)(point.z / Constants.HEX_HEIGHT);
            return result;
        }

        public static List<Point2Int> GetHexInRange(Point2Int origin, int radius)
        {
            var cube = OffsetToCube(origin);
            List<Point2Int> results = new List<Point2Int>();
            for (int q = -radius + cube.q; q <= +radius + cube.q; q++)
            {
                for (int r = -radius + cube.r; r <= +radius + cube.r; r++)
                {
                    for (int s = -radius + cube.s; s <= +radius + cube.s; s++)
                    {
                        if (q + r + s == 0)
                        {
                            results.Add(GridHelpers.CubeToOffset(q, r, s));
                        }
                    }
                }
            }

            return results;
        }

        public static List<Point2Int> GetHexRing(Point2Int origin, int radius)
        {
            return GetHexInRange(origin, radius).Except(GetHexInRange(origin, radius - 1)).ToList();
        }

        public static void SortHexByAngle(List<Point2Int> values, Point2Int origin, bool clockwise)
        {
            values.Sort((Point2Int a, Point2Int b) =>
            {
                float angle = System.MathF.Atan2(a.y - origin.y, a.x - origin.x) - System.MathF.Atan2(b.y - origin.y, b.x - origin.x);
                if (clockwise)
                {
                    angle = -angle;
                }
                if (angle < 0)
                {
                    return -1;
                }
                else if (angle > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });
        }

        public static CubeCoord Rotate60(CubeCoord coord, int rotation, bool clockwise = true)
        {
            int invert = rotation % 2 == 1 ? -1 : 1;
            rotation = rotation % 3;
            var points = new int[] { invert * coord.q, invert * coord.r, invert * coord.s };
            var rotated = clockwise
                ? points.Skip(rotation).Concat(points.Take(rotation)).ToArray()
                : points.Skip((points.Length - rotation) % points.Length).Concat(points.Take((points.Length - rotation) % points.Length)).ToArray();
            return new CubeCoord(rotated[0], rotated[1], rotated[2]);
        }
    }
}