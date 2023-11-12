using System.Collections.Generic;
using System.Data;
using Core;

public struct TerrainPoint
{
    public Triangle[] Traingles;
}

namespace Core
{
    public class Terrain
    {
        public readonly TerrainPoint?[,,] TerrainData;
        public int MaxX => this.TerrainData.GetLength(0);
        public int MaxY => this.TerrainData.GetLength(1);
        public int MaxZ => this.TerrainData.GetLength(2);

        public Terrain(TriangleType?[,,] Types)
        {
            TerrainData = new TerrainPoint?[Types.GetLength(0), Types.GetLength(1), Types.GetLength(2)];

            for (int x = 0; x < Types.GetLength(0); x++)
            {
                for (int y = 0; y < Types.GetLength(1); y++)
                {
                    for (int z = 0; z < Types.GetLength(2); z++)
                    {
                        if (Types[x, y, z] != null)
                        {
                            var triangles = new Triangle[6];
                            for (int i = 0; i < 6; i++)
                            {
                                triangles[i] = new Triangle()
                                {
                                    Type = Types[x, y, z]!.Value,
                                    SubType = TriangleData.AvailableSubTypes[Types[x, y, z]!.Value][0],
                                };
                            }

                            TerrainData[x, y, z] = new TerrainPoint()
                            {
                                Traingles = triangles
                            };
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

        public Terrain(TerrainPoint?[,,] TerrainData)
        {
            this.TerrainData = TerrainData;
        }

        public Schema.Terrain ToSchema()
        {
            // TODO: Implement
            return new Schema.Terrain();
        }

        public TerrainPoint? GetAt(Point3Int location)
        {
            return TerrainData[location.x, location.y, location.z];
        }

        // private void CalculateInitialUncovered()
        // {
        //     if (!GridHelpers.IsInBounds(0, 0, this.MaxHeight - 1, this.Hexes))
        //     {
        //         return;
        //     }

        //     HashSet<Point3Int> visited = new HashSet<Point3Int>();
        //     Queue<Point3Int> queue = new Queue<Point3Int>();

        //     queue.Enqueue(new Point3Int(0, 0, this.MaxHeight - 1));

        //     while (queue.Count > 0)
        //     {
        //         Point3Int current = queue.Dequeue();
        //         if (Hexes[current.x, current.y, current.z] != null)
        //         {
        //             if (this.UncoveredHexes[current.x, current.y] == null)
        //             {
        //                 this.UncoveredHexes[current.x, current.y] = new HashSet<int>();
        //             }

        //             this.UncoveredHexes[current.x, current.y].Add(current.z);
        //         }

        //         Hex? hex = Hexes[current.x, current.y, current.z];
        //         if (hex == null || hex.Transparent)
        //         {
        //             for (int i = 0; i < 8; i++)
        //             {
        //                 Point3Int neighbor = GridHelpers.GetNeighbor(current, (HexSide)i);
        //                 if (!GridHelpers.IsInBounds(neighbor, this.Hexes))
        //                 {
        //                     continue;
        //                 }

        //                 if (visited.Contains(neighbor))
        //                 {
        //                     continue;
        //                 }

        //                 visited.Add(neighbor);
        //                 queue.Enqueue(neighbor);
        //             }
        //         }
        //     }
        // }

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
    }
}
