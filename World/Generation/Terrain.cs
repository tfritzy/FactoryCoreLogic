using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core;
using Newtonsoft.Json;

namespace Core
{
    public class Terrain
    {
        public readonly Triangle?[]?[,,] TerrainData;
        public int MaxX => this.TerrainData.GetLength(0);
        public int MaxY => this.TerrainData.GetLength(1);
        public int MaxZ => this.TerrainData.GetLength(2);

        private Context context;

        public Terrain(TriangleType?[,,] Types, Context context)
        {
            this.context = context;
            TerrainData = new Triangle?[]?[Types.GetLength(0), Types.GetLength(1), Types.GetLength(2)];

            for (int x = 0; x < Types.GetLength(0); x++)
            {
                for (int y = 0; y < Types.GetLength(1); y++)
                {
                    for (int z = 0; z < Types.GetLength(2); z++)
                    {
                        if (Types[x, y, z] != null)
                        {
                            var triangles = new Triangle?[6];
                            for (int i = 0; i < 6; i++)
                            {
                                triangles[i] = new Triangle()
                                {
                                    Type = Types[x, y, z]!.Value,
                                    SubType = TriangleData.AvailableSubTypes[Types[x, y, z]!.Value][0],
                                };
                            }

                            TerrainData[x, y, z] = triangles;
                        }
                    }
                }
            }

            RemoveNonExposedHexes();
            CategorizeTerrain();
        }

        private void CategorizeTerrain()
        {
            for (int x = 0; x < TerrainData.GetLength(0); x++)
            {
                for (int y = 0; y < TerrainData.GetLength(1); y++)
                {
                    for (int z = 0; z < TerrainData.GetLength(2); z++)
                    {
                        if (TerrainData[x, y, z] != null)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                var tri = TerrainData[x, y, z]![i];
                                if (tri != null)
                                {
                                    tri.SubType = ClassifyTri(new Point3Int(x, y, z), (HexSide)i);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RemoveNonExposedHexes()
        {
            HashSet<Point3Int> visited = new();
            Queue<Point3Int> queue = new();
            queue.Enqueue(new Point3Int(0, 0, MaxZ - 1));
            bool[,,] exposed = new bool[MaxX, MaxY, MaxZ];
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (visited.Contains(current))
                {
                    continue;
                }

                if (!IsInBounds(current))
                {
                    continue;
                }

                visited.Add(current);

                exposed[current.x, current.y, current.z] = true;
                for (int i = 0; i < 6; i++)
                {
                    var neighbor = GridHelpers.GetNeighbor(current, (HexSide)i);
                    if (IsInBounds(neighbor))
                    {
                        exposed[neighbor.x, neighbor.y, neighbor.z] = true;
                    }
                }

                var hex = TerrainData[current.x, current.y, current.z];
                if (hex == null || hex[0] == null || hex[0]?.Type == TriangleType.Water)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        var neighbor = GridHelpers.GetNeighbor(current, (HexSide)i);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            for (int x = 0; x < exposed.GetLength(0); x++)
            {
                for (int y = 0; y < exposed.GetLength(1); y++)
                {
                    for (int z = 0; z < exposed.GetLength(2); z++)
                    {
                        if (!exposed[x, y, z])
                        {
                            TerrainData[x, y, z] = null;
                        }
                    }
                }
            }
        }

        public bool IsInBounds(Point2Int point)
        {
            return IsInBounds(point.x, point.y, 0);
        }

        public bool IsInBounds(Point3Int point)
        {
            return IsInBounds(point.x, point.y, point.z);
        }

        public bool IsInBounds(int x, int y, int z)
        {
            if (x < 0 || x >= TerrainData.GetLength(0))
            {
                return false;
            }

            if (y < 0 || y >= TerrainData.GetLength(1))
            {
                return false;
            }

            if (z < 0 || z >= TerrainData.GetLength(2))
            {
                return false;
            }

            return true;
        }

        public Terrain(Triangle?[]?[,,] TerrainData, Context context)
        {
            this.TerrainData = TerrainData;
            this.context = context;
        }

        public Schema.Terrain ToSchema()
        {
            return new Schema.Terrain()
            {
                TerrainData = this.TerrainData,
            };
        }

        public Triangle?[]? GetAt(Point3Int location)
        {
            if (!IsInBounds(location))
            {
                return null;
            }

            return TerrainData[location.x, location.y, location.z];
        }

        public Triangle? GetTri(Point3Int location, HexSide tri)
        {
            var data = TerrainData[location.x, location.y, location.z];
            if (data == null || data.Length == 0)
            {
                return null;
            }

            return data[(int)tri];
        }

        public void SetTriangle(Point3Int location, Triangle? triangle, HexSide side)
        {
            if (TerrainData[location.x, location.y, location.z] == null)
            {
                TerrainData[location.x, location.y, location.z] = new Triangle[6];
            }
            TerrainData[location.x, location.y, location.z]![(int)side] = triangle;

            if (triangle != null)
            {
                context.World.UnseenUpdates.AddLast(new TriUncoveredOrAdded(location, side));
            }
            else
            {
                context.World.UnseenUpdates.AddLast(new TriHiddenOrDestroyed(location, side));
            }
        }

        public Point3Int GetTopHex(Point2Int location)
        {
            for (int z = this.MaxZ - 1; z >= 0; z--)
            {
                if (this.TerrainData[location.x, location.y, z] != null)
                {
                    return new Point3Int(location.x, location.y, z);
                }
            }

            return new Point3Int(location.x, location.y, 0);
        }

        public bool IsTopHexSolid(Point2Int col)
        {
            Point3Int topHex = GetTopHex(col);
            var hex = GetAt(topHex);
            if (hex == null)
            {
                return false;
            }

            for (int i = 0; i < hex.Length; i++)
            {
                if (hex[i] == null)
                {
                    return false;
                }
            }

            return true;
        }


        public TriangleSubType ClassifyTri(Point3Int point, HexSide tri)
        {
            if (GetTri(point, tri)?.Type == TriangleType.Water)
            {
                return TriangleSubType.Liquid;
            }

            // If the triangle opposite this triangle's face is water, it's an outy.
            if (IsOppositeTriFaceWaterOrAir(point, tri))
            {
                return TriangleSubType.LandOuty;
            }

            // If the traingle opposite this traingle's cornor is water, it's an inny.
            var innyCheck = IsOppositeTriCornerWaterOrAir(point, tri);
            if (innyCheck != null)
            {
                return innyCheck.Value;
            }

            // Otherwise it's land locked.
            return TriangleSubType.LandFull;
        }

        private bool IsOppositeTriFaceWaterOrAir(Point3Int point, HexSide tri)
        {
            Point3Int opposite = GridHelpers.GetNeighbor(point, tri);
            if (!IsInBounds(opposite))
            {
                return true;
            }

            var oppositeHex = GetAt(opposite);
            if (oppositeHex == null || oppositeHex[(int)GridHelpers.OppositeSide(tri)] == null)
            {
                return true;
            }

            if (oppositeHex[(int)GridHelpers.OppositeSide(tri)]!.Type == TriangleType.Water)
            {
                return true;
            }

            return false;
        }

        private TriangleSubType? IsOppositeTriCornerWaterOrAir(Point3Int point, HexSide tri)
        {
            bool leftOpen = false;
            bool rightOpen = false;

            HexSide counterClockwise = GridHelpers.Rotate60(tri, clockwise: false);
            HexSide clockwise = GridHelpers.Rotate60(tri, clockwise: true);

            var hexOppositeCounterClockwise = GetAt(GridHelpers.GetNeighbor(point, counterClockwise));
            if (hexOppositeCounterClockwise != null)
            {
                TriangleType? relevantSide = hexOppositeCounterClockwise[(int)clockwise]?.Type;
                if (relevantSide == null || relevantSide == TriangleType.Water)
                {
                    leftOpen = true;
                }
            }
            else
            {
                leftOpen = true;
            }

            var hexOppositeClockwise = GetAt(GridHelpers.GetNeighbor(point, clockwise));
            if (hexOppositeClockwise != null)
            {
                TriangleType? relevantSide = hexOppositeClockwise[(int)counterClockwise]?.Type;
                if (relevantSide == null || relevantSide == TriangleType.Water)
                {
                    rightOpen = true;
                }
            }
            else
            {
                rightOpen = true;
            }

            if (leftOpen && rightOpen)
                return TriangleSubType.LandInnyBoth;
            else if (leftOpen)
                return TriangleSubType.LandInnyLeft;
            else if (rightOpen)
                return TriangleSubType.LandInnyRight;
            else
                return null;
        }
    }
}
