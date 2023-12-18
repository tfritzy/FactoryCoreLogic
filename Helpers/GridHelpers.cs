using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public static class GridHelpers
    {
        private static readonly Point2Int[] oddNeighborPattern = new Point2Int[]
        {
            new Point2Int(0, 1), // northeast
            new Point2Int(1, 0), // east
            new Point2Int(0, -1), // southeast
            new Point2Int(-1, -1), // southwest
            new Point2Int(-1, 0), // west
            new Point2Int(-1, 1), // northwest
        };

        private static readonly Point2Int[] evenNeighborPattern = new Point2Int[]
        {
            new Point2Int(1, 1), // northeast
            new Point2Int(1, 0), // east
            new Point2Int(1, -1), // southeast 
            new Point2Int(0, -1), // southwest 
            new Point2Int(-1, 0), // west 
            new Point2Int(0, 1), // northwest 
            
        };

        private static readonly Dictionary<Point2Int, HexSide> oddNeighborPatternMap = new Dictionary<Point2Int, HexSide>()
        {
            { oddNeighborPattern[0], HexSide.NorthEast },
            { oddNeighborPattern[1], HexSide.East },
            { oddNeighborPattern[2], HexSide.SouthEast },
            { oddNeighborPattern[3], HexSide.SouthWest },
            { oddNeighborPattern[4], HexSide.West },
            { oddNeighborPattern[5], HexSide.NorthWest }
        };

        private static readonly Dictionary<Point2Int, HexSide> evenNeighborPatternMap = new Dictionary<Point2Int, HexSide>()
        {
            { evenNeighborPattern[0], HexSide.NorthEast },
            { evenNeighborPattern[1], HexSide.East },
            { evenNeighborPattern[2], HexSide.SouthEast },
            { evenNeighborPattern[3], HexSide.SouthWest },
            { evenNeighborPattern[4], HexSide.West },
            { evenNeighborPattern[5], HexSide.NorthWest }
        };

        public static Point2Int GetNeighbor(int x, int y, HexSide direction)
        {
            return GetNeighbor(new Point2Int(x, y), direction);
        }

        public static Point2Int GetNeighbor(Point2Int pos, HexSide direction)
        {
            Point2Int position;
            int dir = Math.Abs((int)direction) % 6;

            if (Math.Abs(pos.y) % 2 == 0)
            {
                position = pos + evenNeighborPattern[dir];
            }
            else
            {
                position = pos + oddNeighborPattern[dir];
            }

            return position;
        }

        public static Point3Int GetNeighbor(Point3Int pos, HexSide direction)
        {
            if (direction == HexSide.Up)
            {
                return pos + Point3Int.Up;
            }
            else if (direction == HexSide.Down)
            {
                return pos + Point3Int.Down;
            }
            else
            {
                var neighbor = GetNeighbor((Point2Int)pos, direction);
                return new Point3Int(neighbor.x, neighbor.y, pos.z);
            }
        }

        public static HexSide? GetNeighborSide(Point2Int pos, Point2Int neighborPos)
        {
            Point2Int direction = neighborPos - pos;

            if (Math.Abs(pos.y) % 2 == 0)
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

        public static Point2Int CubeToEvenR(CubeCoord cube)
        {
            return CubeToEvenR(cube.q, cube.r, cube.s);
        }

        public static Point2Int CubeToEvenR(int q, int r, int s)
        {
            int col = q + (r + (r & 1)) / 2;
            int row = r;
            return new Point2Int(col, row);
        }

        public static CubeCoord EvenRToCube(Point2Int point)
        {
            int q = point.x - (point.y + (point.y & 1)) / 2;
            int r = point.y;
            int s = -q - r;
            return new CubeCoord(q, r, s);
        }

        /*
            Stolen from: https://www.redblobgames.com/grids/hexagons/more-pixel-to-hex.html#mark-steere
        */
        public static Point2Int pixel_to_evenr_offset(Point2Float point)
        {
            /*
            // Convert to their coordinate system
            y = -y
            // Algorithm from Mark Steere
            let s_to_s = sqrt(3) // "side to side" distance, "w" on my page
            let u = (sqrt(3) * y - x) / 2
            let v = (sqrt(3) * y + x) / 2
            let x_halfcell = floor(2 * x / s_to_s)
            let u_halfcell = floor(2 * u / s_to_s)
            let v_halfcell = floor(2 * v / s_to_s)
            let W = floor((x_halfcell + v_halfcell + 2) / 3)
            let Y = floor((u_halfcell + v_halfcell + 2) / 3)
            return {q: W, r: -Y}
            */
            float s_to_s = MathF.Sqrt(3);
            float u = (MathF.Sqrt(3) * point.y - point.x) / 2;
            float v = (MathF.Sqrt(3) * point.y + point.x) / 2;
            float x_halfcell = MathF.Floor(2 * point.x / s_to_s);
            float u_halfcell = MathF.Floor(2 * u / s_to_s);
            float v_halfcell = MathF.Floor(2 * v / s_to_s);
            float W = MathF.Floor((x_halfcell + v_halfcell + 2) / 3);
            float Y = MathF.Floor((u_halfcell + v_halfcell + 2) / 3);
            var cube = new CubeCoord((int)W, (int)-Y);
            return CubeToEvenR(cube);
        }

        public static HexSide pixel_hex_side(Point2Float point)
        {
            Point2Int evenR = pixel_to_evenr_offset(point);
            Point2Float pixel = evenr_offset_to_pixel(evenR);
            point.x -= pixel.x;
            point.y -= pixel.y;
            // Algorithm from Mark Steere
            float s_to_s = MathF.Sqrt(3);
            float u = (MathF.Sqrt(3) * point.y - point.x) / 2;
            float v = (MathF.Sqrt(3) * point.y + point.x) / 2;
            float x_halfcell = MathF.Floor(2 * point.x / s_to_s);
            float u_halfcell = MathF.Floor(2 * u / s_to_s);
            float v_halfcell = MathF.Floor(2 * v / s_to_s);

            if (x_halfcell == 0 && u_halfcell == 0 && v_halfcell == 0)
            {
                return HexSide.NorthEast;
            }
            else if (x_halfcell == 0 && u_halfcell == -1 && v_halfcell == 0)
            {
                return HexSide.East;
            }
            else if (x_halfcell == 0 && u_halfcell == -1 && v_halfcell == -1)
            {
                return HexSide.SouthEast;
            }
            else if (x_halfcell == -1 && u_halfcell == -1 && v_halfcell == -1)
            {
                return HexSide.SouthWest;
            }
            else if (x_halfcell == -1 && u_halfcell == 0 && v_halfcell == -1)
            {
                return HexSide.West;
            }
            else if (x_halfcell == -1 && u_halfcell == 0 && v_halfcell == 0)
            {
                return HexSide.NorthWest;
            }
            else
            {
                throw new Exception("Invalid hex side");
            }

        }

        public static Point3Int PixelToEvenRPlusHeight(Point3Float point)
        {
            Point2Int evenR = pixel_to_evenr_offset((Point2Float)point);
            return new Point3Int(evenR.x, evenR.y, (int)Math.Round(point.z / Constants.HEX_HEIGHT));
        }

        public static Point3Float EvenRToPixelPlusHeight(Point3Int hex)
        {
            Point2Float evenR = evenr_offset_to_pixel((Point2Int)hex);
            return new Point3Float(evenR.x, evenR.y, hex.z * Constants.HEX_HEIGHT);
        }

        public static Point2Float evenr_offset_to_pixel(Point2Int hex)
        {
            float x = Constants.HEX_RADIUS * MathF.Sqrt(3) * (hex.x - 0.5f * (hex.y & 1));
            float y = Constants.HEX_RADIUS * 3 / 2 * hex.y;
            y = -y;
            return new Point2Float(x, y);
        }

        public static List<Point2Int> GetHexInRange(Point2Int origin, int radius)
        {
            var cube = EvenRToCube(origin);
            List<Point2Int> results = new List<Point2Int>();
            for (int q = -radius + cube.q; q <= +radius + cube.q; q++)
            {
                for (int r = -radius + cube.r; r <= +radius + cube.r; r++)
                {
                    for (int s = -radius + cube.s; s <= +radius + cube.s; s++)
                    {
                        if (q + r + s == 0)
                        {
                            results.Add(GridHelpers.CubeToEvenR(q, r, s));
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

        public static HexSide Rotate60(HexSide side, int numSteps = 1, bool clockwise = true)
        {
            int dir = clockwise ? 1 : -1;
            int final = (int)side + dir * numSteps;
            while (final < 0)
                final += 6;
            return (HexSide)(final % 6);
        }
    }
}