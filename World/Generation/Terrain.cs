using System;
using System.Collections.Generic;
using System.Linq;
using Noise;
using Schema;

namespace Core
{
    public class Terrain
    {
        public readonly Triangle?[]?[,,] TerrainData;
        public int MaxX => TerrainData.GetLength(0);
        public int MaxY => TerrainData.GetLength(1);
        public int MaxZ => TerrainData.GetLength(2);
        public readonly TerrainObject?[,] TerrainObjects;
        public Point3Float MaxBounds;
        public Point3Float MinBounds;

        private Context context;

        public Terrain(Schema.Terrain schema, Context context) : this(ParseTerrainData(schema), ParseTerrainObjects(schema), context)
        {
        }

        public Terrain(Triangle?[]?[,,] terrainData, TerrainObject?[,] terrainObjects, Context context)
        {
            this.TerrainData = terrainData;
            this.context = context;
            TerrainObjects = terrainObjects;
        }

        public Terrain(TriangleType?[,,] Types, Context context)
        {
            this.context = context;
            TerrainData = new Triangle?[]?[Types.GetLength(0), Types.GetLength(1), Types.GetLength(2)];
            TerrainObjects = new TerrainObject?[Types.GetLength(0), Types.GetLength(1)];

            MaxBounds = new Point3Float(
                (MaxX + 1) * Constants.HEX_WIDTH,
                (MaxY + 1) * Constants.HEX_LENGTH,
                (MaxZ + 1) * Constants.HEX_HEIGHT);
            MinBounds = new Point3Float(
                -1 * Constants.HEX_WIDTH,
                -1 * Constants.HEX_LENGTH,
                -1 * Constants.HEX_HEIGHT);

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
                                triangles[i] = new Triangle
                                {
                                    Type = Types[x, y, z]!.Value,
                                    SubType = TriangleData.AvailableSubTypes[Types[x, y, z]!.Value][0]
                                };
                            }

                            TerrainData[x, y, z] = triangles;
                        }
                    }
                }
            }

            RemoveNonExposedHexes();
            CategorizeTerrain();
            PopulateVegetation();
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

        private void PopulateVegetation()
        {
            Random r = new Random();
            OpenSimplexNoise birchNoise = new OpenSimplexNoise(r.Next());
            OpenSimplexNoise pineNoise = new OpenSimplexNoise(r.Next());
            OpenSimplexNoise treeThinningNoise = new OpenSimplexNoise(r.Next());

            for (int x = 0; x < TerrainObjects.GetLength(0); x++)
            {
                for (int y = 0; y < TerrainObjects.GetLength(1); y++)
                {
                    double birchVal = birchNoise.Evaluate(x / 2f, y / 2f);
                    birchVal = (birchVal + 1) / 2;
                    double pineVal = pineNoise.Evaluate(x / 2f, y / 40f);
                    pineVal = (pineVal + 1) / 2;

                    double treeThinningVal = treeThinningNoise.Evaluate(x * .5f, y * .5f);
                    treeThinningVal = ((treeThinningVal + 1) / 2) * .2f;
                    birchVal -= treeThinningVal;
                    pineVal -= treeThinningVal;

                    double bushVal = r.NextDouble();
                    double mushroomValue = r.NextDouble();
                    if (birchVal > 0.8)
                    {
                        TerrainObjects[x, y] = new TerrainObject(TerrainObjectType.BirchTree);
                    }
                    else if (pineVal > 0.8)
                    {
                        TerrainObjects[x, y] = new TerrainObject(TerrainObjectType.PineTree);
                    }
                    else if (bushVal > 0.99)
                    {
                        TerrainObjects[x, y] = new TerrainObject(TerrainObjectType.Bush);
                    }
                    else if (mushroomValue > 0.98)
                    {
                        TerrainObjects[x, y] = new TerrainObject(TerrainObjectType.Mushroom);
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

        public bool IsInBounds(Point3Float point)
        {
            if (point.x < MinBounds.x || point.x > MaxBounds.x)
            {
                return false;
            }

            if (point.y < MinBounds.y || point.y > MaxBounds.y)
            {
                return false;
            }

            if (point.z < MinBounds.z || point.z > MaxBounds.z)
            {
                return false;
            }

            return true;
        }


        private static Triangle?[]?[,,] ParseTerrainData(Schema.Terrain schema)
        {
            Triangle?[]?[,,] data = new Triangle?[]?[schema.XLength, schema.YLength, schema.ZLength];
            for (int x = 0; x < schema.XLength; x++)
            {
                for (int y = 0; y < schema.YLength; y++)
                {
                    for (int z = 0; z < schema.ZLength; z++)
                    {
                        Schema.NullableHex hex = schema.FlatTerrainData[
                            x * schema.YLength * schema.ZLength + y * schema.ZLength + z];
                        switch (hex.ValueCase)
                        {
                            case NullableHex.ValueOneofCase.Hex:
                                data[x, y, z] = hex.Hex.Tris.ToArray();
                                break;
                            case NullableHex.ValueOneofCase.NullValue:
                                break;
                            case NullableHex.ValueOneofCase.None:
                                break;
                        }
                    }
                }
            }

            return data;
        }

        private static TerrainObject?[,] ParseTerrainObjects(Schema.Terrain schema)
        {
            TerrainObject?[,] data = new TerrainObject?[schema.XLength, schema.YLength];
            for (int x = 0; x < schema.XObjectLength; x++)
            {
                for (int y = 0; y < schema.YObjectLength; y++)
                {
                    NullableTerrainObject obj = schema.Objects[x * schema.YObjectLength + y];
                    switch (obj.ValueCase)
                    {
                        case NullableTerrainObject.ValueOneofCase.Object:
                            data[x, y] = TerrainObject.FromSchema(obj.Object);
                            break;
                        case NullableTerrainObject.ValueOneofCase.NullValue:
                            break;
                        case NullableTerrainObject.ValueOneofCase.None:
                            break;
                    }
                }
            }

            return data;
        }

        private static Schema.NullableHex[] FlattenTerrainData(Triangle?[]?[,,] terrainData)
        {
            Schema.NullableHex[] data = new Schema.NullableHex[
                terrainData.GetLength(0) * terrainData.GetLength(1) * terrainData.GetLength(2)];
            for (int x = 0; x < terrainData.GetLength(0); x++)
            {
                for (int y = 0; y < terrainData.GetLength(1); y++)
                {
                    for (int z = 0; z < terrainData.GetLength(2); z++)
                    {
                        if (terrainData[x, y, z] == null)
                        {
                            data[
                                x * terrainData.GetLength(1) * terrainData.GetLength(2) +
                                y * terrainData.GetLength(2) + z] =
                                    new Schema.NullableHex { NullValue = new() };
                        }
                        else
                        {
                            var hex = new Schema.Hex();
                            hex.Tris.AddRange(terrainData[x, y, z]!);
                            data[
                                x * terrainData.GetLength(1) * terrainData.GetLength(2) +
                                y * terrainData.GetLength(2) + z] = new Schema.NullableHex { Hex = hex };
                        }
                    }
                }
            }

            return data;
        }

        private static Schema.NullableTerrainObject[] FlattenTerrainObjects(TerrainObject?[,] terrainObjects)
        {
            Schema.NullableTerrainObject[] data = new Schema.NullableTerrainObject[
                terrainObjects.GetLength(0) * terrainObjects.GetLength(1)];
            for (int x = 0; x < terrainObjects.GetLength(0); x++)
            {
                for (int y = 0; y < terrainObjects.GetLength(1); y++)
                {
                    if (terrainObjects[x, y] == null)
                    {
                        data[x * terrainObjects.GetLength(1) + y] =
                            new Schema.NullableTerrainObject { NullValue = new() };
                    }
                    else
                    {
                        data[x * terrainObjects.GetLength(1) + y] =
                            new Schema.NullableTerrainObject
                            {
                                Object = terrainObjects[x, y]!.ToSchema()
                            };
                    }
                }
            }

            return data;
        }

        public Schema.Terrain ToSchema()
        {
            return new Schema.Terrain()
            {
                FlatTerrainData = { FlattenTerrainData(TerrainData) },
                XLength = MaxX,
                YLength = MaxY,
                ZLength = MaxZ,

                Objects = { FlattenTerrainObjects(TerrainObjects) },
                XObjectLength = TerrainObjects.GetLength(0),
                YObjectLength = TerrainObjects.GetLength(1),
            };
        }

        public static Terrain FromSchema(Schema.Terrain schema, params object[] context)
        {
            return new Terrain(schema, (Context)context[0]);
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
            if (triangle != null)
            {
                context.World.AddUpdateForFrame(
                    new OneofUpdate
                    {
                        TriUncoveredOrAdded = new TriUncoveredOrAdded
                        {
                            GridPosition = location.ToSchema(),
                            Side = side,
                            Tri = triangle,
                        }
                    });
            }
            else
            {
                context.World.AddUpdateForFrame(
                    new OneofUpdate
                    {
                        TriHiddenOrDestroyed = new TriHiddenOrDestroyed
                        {
                            GridPosition = location.ToSchema(),
                            Side = side,
                        }
                    });
            }
        }

        public Point3Int GetTopHex(Point2Int location, HexSide? side = null)
        {
            for (int z = this.MaxZ - 1; z >= 0; z--)
            {
                var hex = this.TerrainData[location.x, location.y, z];
                if (hex != null)
                {
                    if (side == null)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (hex[i] != null)
                            {
                                return new Point3Int(location.x, location.y, z);
                            }
                        }
                    }
                    else
                    {
                        if (hex[(int)side] != null)
                        {
                            return new Point3Int(location.x, location.y, z);
                        }
                    }
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

        public TerrainObject? GetTerrainObject(Point2Int pos)
        {
            if (!IsInBounds(pos))
            {
                return null;
            }

            return TerrainObjects[pos.x, pos.y];
        }
    }
}
