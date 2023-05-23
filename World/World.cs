using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Core
{
    public class World : Schema.SerializesTo<Schema.World>
    {
        private Hex?[,,] Hexes;
        private Dictionary<Point2Int, ulong> Buildings;
        public List<ulong> Villagers { get; private set; }
        private Dictionary<ulong, Character> Characters;
        public LinkedList<Update> UnseenUpdates = new LinkedList<Update>();

        public class Update
        {
            public Point2Int Location { get; private set; }
            public ulong Character { get; private set; }

            public Update(ulong character)
            {
                this.Character = character;
            }

            public Update(int x, int y)
            {
                this.Location = new Point2Int(x, y);
            }

            public Update(Point2Int location)
            {
                this.Location = location;
            }
        }

        public int MaxX => Hexes.GetLength(0);
        public int MaxY => Hexes.GetLength(1);
        public int MaxHeight => Hexes.GetLength(2);

        private HashSet<int>[,] UncoveredHexes;

        public World() : this(new Hex?[0, 0, 0]) { }

        public World(Hex?[,,] hexes)
        {
            this.Characters = new Dictionary<ulong, Character>();
            this.Buildings = new Dictionary<Point2Int, ulong>();
            this.Villagers = new List<ulong>();
            this.Hexes = hexes;
            this.UncoveredHexes = new HashSet<int>[hexes.GetLength(0), hexes.GetLength(1)];
            CalculateInitialUncovered();
        }

        public void SetHexes(Hex?[,,] hexes)
        {
            this.Hexes = hexes;
            this.UncoveredHexes = new HashSet<int>[hexes.GetLength(0), hexes.GetLength(1)];
            CalculateInitialUncovered();
        }

        public void Tick(float deltaTime)
        {
            foreach (ulong buildingId in this.Buildings.Values)
            {
                Characters[buildingId].Tick(deltaTime);
            }
        }

        public Hex? GetTopHex(Point2Int point)
        {
            return GetTopHex(point.x, point.y);
        }

        public Hex? GetTopHex(int x, int y)
        {
            int? topHeight = GetTopHexHeight(x, y);

            if (topHeight == null)
            {
                return null;
            }

            return GetHex(x, y, topHeight.Value);
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

        public void SetHex(Hex hex)
        {
            if (!GridHelpers.IsInBounds(hex.GridPosition, this.Hexes))
            {
                return;
            }

            this.UnseenUpdates.AddLast(new Update((Point2Int)hex.GridPosition));
            this.Hexes[hex.GridPosition.x, hex.GridPosition.y, hex.GridPosition.z] = hex;
        }

        public int GetTopHexHeight(Point2Int point)
        {
            return GetTopHexHeight(point.x, point.y);
        }

        public int GetTopHexHeight(int x, int y)
        {
            if (!GridHelpers.IsInBounds(x, y, 0, this.Hexes) || this.UncoveredHexes[x, y] == null)
            {
                throw new System.ArgumentException("Invalid hex column");
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
            if (!GridHelpers.IsInBounds(0, 0, this.MaxHeight - 1, this.Hexes))
            {
                return;
            }

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

                Hex? hex = Hexes[current.x, current.y, current.z];
                if (hex == null || hex.Transparent)
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

        private void MarkNeighborsUncovered(Point3Int point)
        {
            for (int i = 0; i < 8; i++)
            {
                Point3Int neighborPos = GridHelpers.GetNeighbor(point, (HexSide)i);

                Hex? neighbor = GetHex(neighborPos);
                if (neighbor != null)
                {
                    if (this.UncoveredHexes[neighborPos.x, neighborPos.y] == null)
                    {
                        this.UncoveredHexes[neighborPos.x, neighborPos.y] = new HashSet<int>();
                    }

                    if (!this.UncoveredHexes[neighborPos.x, neighborPos.y].Contains(neighborPos.z))
                    {
                        this.UncoveredHexes[neighborPos.x, neighborPos.y].Add(neighborPos.z);
                        this.UnseenUpdates.AddLast(new Update(neighborPos.x, neighborPos.y));
                    }
                }
            }
        }

        public void RemoveHex(Point3Int location)
        {
            this.Hexes[location.x, location.y, location.z] = null;
            this.UncoveredHexes[location.x, location.y].Remove(location.z);
            this.UnseenUpdates.AddLast(new Update(location.x, location.y));
            MarkNeighborsUncovered(location);
        }

        public void AddCharacter(Character character)
        {
            if (character is Villager)
            {
                this.Villagers.Add(character.Id);
            }

            this.Characters[character.Id] = character;
            this.UnseenUpdates.AddLast(new Update(character.Id));
        }

        public void RemoveCharacter(ulong id)
        {
            if (this.Characters.ContainsKey(id) && this.Characters[id] is Villager)
            {
                this.Villagers.Remove(id);
            }

            if (!this.Characters.ContainsKey(id))
            {
                return;
            }

            this.Characters.Remove(id);
            this.UnseenUpdates.AddLast(new Update(id));
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

            this.UnseenUpdates.AddLast(new Update(location));
        }

        public void RemoveBuilding(Point2Int location)
        {
            ulong buildingId = this.Buildings[location];
            Building building = (Building)this.Characters[buildingId];
            this.Buildings.Remove(location);
            building.OnRemoveFromGrid();

            this.UnseenUpdates.AddLast(new Update(location));
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

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this.ToSchema(), new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
            });
        }

        public Schema.World ToSchema()
        {
            Schema.Hex?[,,] hexes = new Schema.Hex?[
                Hexes.GetLength(0),
                Hexes.GetLength(1),
                Hexes.GetLength(2)];
            for (int x = 0; x < Hexes.GetLength(0); x++)
            {
                for (int y = 0; y < Hexes.GetLength(1); y++)
                {
                    for (int z = 0; z < Hexes.GetLength(2); z++)
                    {
                        hexes[x, y, z] = Hexes[x, y, z]?.ToSchema();
                    }
                }
            }

            return new Schema.World
            {
                Hexes = hexes,
                Buildings = this.Buildings
                    .Where(kvp => !this.Characters[kvp.Value].IsPreview)
                    .ToDictionary(
                        kvp => new Point2Int(kvp.Key.x, kvp.Key.y),
                        kvp => kvp.Value),
                Characters = this.Characters
                    .Where(kvp => !kvp.Value.IsPreview)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.ToSchema()),
            };
        }

        public static World FromSchema(string text)
        {
            Schema.World? schemaWorld = JsonConvert.DeserializeObject<Schema.World>(text);

            if (schemaWorld == null)
            {
                throw new InvalidOperationException("Failed to deserialize world");
            }

            return schemaWorld.FromSchema();
        }

        public Character? GetCharacter(ulong id)
        {
            if (!this.Characters.ContainsKey(id))
            {
                return null;
            }

            return this.Characters[id];
        }

        public bool TryGetCharacter(ulong id, out Character? character)
        {
            return this.Characters.TryGetValue(id, out character);
        }

        public void AckUpdate()
        {
            this.UnseenUpdates.RemoveFirst();
        }
    }
}