using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FactoryCore
{
    public class World : ISerializable<World>
    {
        public int MaxX => Hexes.GetLength(0);
        public int MaxY => Hexes.GetLength(1);
        public int MaxHeight => Hexes.GetLength(2);

        private HashSet<int>[,] UncoveredHexes;
        private Hex?[,,] Hexes;
        private Dictionary<Point2Int, Building> Buildings;

        public World(Hex?[,,] hexes)
        {
            this.Hexes = hexes;
            this.UncoveredHexes = new HashSet<int>[hexes.GetLength(0), hexes.GetLength(1)];
            this.Buildings = new Dictionary<Point2Int, Building>();
            CalculateInitialUncovered();
        }

        public void Tick(float deltaTime)
        {
            foreach (Building building in this.Buildings.Values)
            {
                building.Tick(deltaTime);
            }
        }

        public int GetTopHexHeight(int x, int y)
        {
            if (this.UncoveredHexes[x, y] == null)
            {
                return -1;
            }

            return this.UncoveredHexes[x, y].Max();
        }

        public bool IsUncovered(Point3Int point)
        {
            if (Hexes[point.x, point.y, point.z] == null)
            {
                return true;
            }

            if (this.UncoveredHexes[point.x, point.y] == null)
            {
                return false;
            }

            return this.UncoveredHexes[point.x, point.y].Contains(point.z);
        }

        private void CalculateInitialUncovered()
        {
            HashSet<Point3Int> visited = new HashSet<Point3Int>();
            Queue<Point3Int> queue = new Queue<Point3Int>();

            queue.Enqueue(new Point3Int(0, 0, this.MaxHeight - 1));

            while (queue.Count > 0)
            {
                Point3Int current = queue.Dequeue();
                if (Hexes[current.x, current.y, current.z] != null)
                {
                    if (this.UncoveredHexes[current.x, current.y] == null)
                    {
                        this.UncoveredHexes[current.x, current.y] = new HashSet<int>();
                    }

                    this.UncoveredHexes[current.x, current.y].Add(current.z);
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Point3Int neighbor = HexGridHelpers.GetNeighbor(current, (HexSide)i);
                        if (!HexGridHelpers.IsInBounds(neighbor, this.Hexes))
                        {
                            continue;
                        }

                        if (visited.Contains(neighbor))
                        {
                            continue;
                        }

                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        public void AddBuilding(Building building, Point2Int location)
        {
            if (Buildings.ContainsKey(location))
            {
                throw new InvalidOperationException("Tried to place building on occupied location");
            }

            this.Buildings.Add(location, building);
            building.OnAddToGrid(location);
        }

        public void RemoveBuilding(Point2Int location)
        {
            Building building = this.Buildings[location];
            this.Buildings.Remove(location);
            building.OnRemoveFromGrid();
        }

        public Building? GetBuildingAt(Point2Int location)
        {
            if (this.Buildings.ContainsKey(location))
            {
                return this.Buildings[location];
            }
            else
            {
                return null;
            }
        }

        public string ToSchema()
        {
            return JsonSerializer.Serialize(this);
        }

        public World FromSchema(string text)
        {
            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyProperties = true
            };
            World? world = JsonSerializer.Deserialize<World>(text, options);

            if (world == null)
            {
                throw new InvalidOperationException("Failed to deserialize world");
            }

            return world;
        }
    }
}