using System.Collections.Generic;
using System.Data;
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
            return TerrainData[location.x, location.y, location.z];
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
    }
}
