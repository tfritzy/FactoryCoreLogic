using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace FactoryCore
{
    [JsonObject(MemberSerialization.OptIn)]
    public class World
    {
        [JsonProperty("hexes")]
        private Hex?[,,] Hexes;

        [JsonProperty("buildings")]
        private Dictionary<Point2Int, ulong> Buildings;

        [JsonProperty("characters")]
        private Dictionary<ulong, Character> Characters;

        public int MaxX => Hexes.GetLength(0);
        public int MaxY => Hexes.GetLength(1);
        public int MaxHeight => Hexes.GetLength(2);

        private HashSet<int>[,] UncoveredHexes;

        public World(Hex?[,,] hexes)
        {
            this.Characters = new Dictionary<ulong, Character>();
            this.Hexes = hexes;
            this.UncoveredHexes = new HashSet<int>[hexes.GetLength(0), hexes.GetLength(1)];
            this.Buildings = new Dictionary<Point2Int, ulong>();
            CalculateInitialUncovered();
        }

        public void Tick(float deltaTime)
        {
            foreach (ulong buildingId in this.Buildings.Values)
            {
                Characters[buildingId].Tick(deltaTime);
            }
        }

        public Hex? GetHex(Point3Int point)
        {
            return GetHex(point.x, point.y, point.z);
        }

        public Hex? GetHex(int x, int y, int z)
        {
            if (!GridHelpers.IsInBounds(x, y, z, this.Hexes))
            {
                return null;
            }

            return this.Hexes[x, y, z];
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
            return IsUncovered(point.x, point.y, point.z);
        }

        public bool IsUncovered(int x, int y, int z)
        {
            if (Hexes[x, y, z] == null)
            {
                return true;
            }

            if (this.UncoveredHexes[x, y] == null)
            {
                return false;
            }

            return this.UncoveredHexes[x, y].Contains(z);
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
                        Point3Int neighbor = GridHelpers.GetNeighbor(current, (HexSide)i);
                        if (!GridHelpers.IsInBounds(neighbor, this.Hexes))
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

        public void AddCharacter(Character character)
        {
            this.Characters[character.Id] = character;
        }

        public void AddBuilding(Building building, Point2Int location)
        {
            if (Buildings.ContainsKey(location))
            {
                throw new InvalidOperationException("Tried to place building on occupied location");
            }

            this.Characters[building.Id] = building;
            this.Buildings.Add(location, building.Id);
            building.OnAddToGrid(location);
        }

        public void RemoveBuilding(Point2Int location)
        {
            ulong buildingId = this.Buildings[location];
            Building building = (Building)this.Characters[buildingId];
            this.Buildings.Remove(location);
            building.OnRemoveFromGrid();
        }

        public Building? GetBuildingAt(int x, int y) => GetBuildingAt(new Point2Int(x, y));
        public Building? GetBuildingAt(Point2Int location)
        {
            if (this.Buildings.ContainsKey(location))
            {
                return (Building)this.Characters[this.Buildings[location]];
            }
            else
            {
                return null;
            }
        }

        public string ToSchema()
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
            };

            return JsonConvert.SerializeObject(this, settings);
        }

        public static World FromSchema(string text)
        {
            Context context = new Context();
            World? world = JsonConvert.DeserializeObject<World>(text, new JsonSerializerSettings
            {
                Context = new StreamingContext(StreamingContextStates.All, context),
            });

            if (world == null)
            {
                throw new InvalidOperationException("Failed to deserialize world");
            }

            context.World = world;

            return world;
        }

        public bool TryGetCharacter(ulong id, out Character? character)
        {
            return this.Characters.TryGetValue(id, out character);
        }
    }
}