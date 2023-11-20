using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Core
{
    public class World
    {
        public Terrain Terrain { get; private set; }
        private Dictionary<Point2Int, ulong> Buildings;
        private Dictionary<ulong, Character> Characters;
        public Dictionary<ulong, Projectile> Projectiles { get; private set; }
        public LinkedList<Update> UnseenUpdates = new LinkedList<Update>();

        public int MaxX => Terrain.MaxX;
        public int MaxY => Terrain.MaxY;
        public int MaxHeight => Terrain.MaxZ;

        [Obsolete("Only to be used during deserialization")]
        public World() : this(new Terrain(new TriangleType?[0, 0, 0], null!)) { }

        public World(Terrain terrain)
        {
            this.Characters = new Dictionary<ulong, Character>();
            this.Buildings = new Dictionary<Point2Int, ulong>();
            this.Projectiles = new Dictionary<ulong, Projectile>();
            this.Terrain = terrain;
        }

        public void SetTerrain(Terrain terrain)
        {
            this.Terrain = terrain;
        }

        public void Tick(float deltaTime)
        {
            for (int i = 0; i < this.Characters.Count; i++)
            {
                ulong characterId = this.Characters.Keys.ElementAt(i);
                Characters[characterId].Tick(deltaTime);
            }

            foreach (Projectile projectile in this.Projectiles.Values)
            {
                projectile.Tick(deltaTime);
            }
        }

        public void AddCharacter(Character character)
        {
            this.Characters[character.Id] = character;
            this.UnseenUpdates.AddLast(new CharacterUpdate(character.Id));
        }

        public void RemoveCharacter(ulong id)
        {
            if (!this.Characters.ContainsKey(id))
            {
                return;
            }

            this.Characters.Remove(id);
            this.UnseenUpdates.AddLast(new CharacterUpdate(id));
        }

        public void AddBuilding(Building building, Point2Int location)
        {
            if (Buildings.ContainsKey((Point2Int)location))
            {
                throw new InvalidOperationException("Tried to place building on occupied location");
            }

            if (!Terrain.IsInBounds(location))
            {
                throw new InvalidOperationException("Tried to place building out of bounds");
            }

            if (!Terrain.IsTopHexSolid(location))
            {
                throw new InvalidOperationException("Must place building on solid ground");
            }

            this.Characters[building.Id] = building;
            this.Buildings.Add((Point2Int)location, building.Id);
            building.OnAddToGrid(location);

            this.UnseenUpdates.AddLast(new BuildingAdded((Point2Int)location));
        }

        public void RemoveBuilding(Point2Int location)
        {
            ulong buildingId = this.Buildings[location];
            Building building = (Building)this.Characters[buildingId];
            this.Buildings.Remove(location);
            building.OnRemoveFromGrid();

            this.UnseenUpdates.AddLast(new BuildingRemoved(buildingId, building.GridPosition));
        }

        public void RemoveBuilding(ulong id)
        {
            if (!this.Characters.ContainsKey(id))
            {
                return;
            }

            var building = this.Characters[id];
            RemoveBuilding((Point2Int)building.GridPosition);
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
            return new Schema.World
            {
                Terrain = Terrain.ToSchema(),
                Buildings = this.Buildings
                    .Where(kvp => !this.Characters[kvp.Value].IsPreview)
                    .ToDictionary(
                        kvp => new Point2Int(kvp.Key.x, kvp.Key.y),
                        kvp => kvp.Value),
                Characters = this.Characters
                    .Where(kvp => !kvp.Value.IsPreview)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => (Schema.Character)kvp.Value.ToSchema()),
            };
        }

        public static World Deserialize(string text)
        {
            Schema.World? schemaWorld = JsonConvert.DeserializeObject<Schema.World>(text);

            if (schemaWorld == null)
            {
                throw new InvalidOperationException("Failed to deserialize world");
            }

            return schemaWorld.FromSchema();
        }

        public Character? FindCharacter(Func<Character, bool> predicate)
        {
            return this.Characters.Values.FirstOrDefault(predicate);
        }

        public List<Character> FindCharacters(Func<Character, bool> predicate)
        {
            return this.Characters.Values.Where(predicate).ToList();
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

        public Projectile? GetProjectile(ulong id)
        {
            if (!this.Projectiles.ContainsKey(id))
            {
                return null;
            }

            return this.Projectiles[id];
        }

        public void AddProjectile(Projectile projectile)
        {
            this.Projectiles.Add(projectile.Id, projectile);
            this.UnseenUpdates.AddLast(new ProjectileAdded(projectile.Id));
        }

        public void RemoveProjectile(ulong id)
        {
            this.Projectiles.Remove(id);
            this.UnseenUpdates.AddLast(new ProjectileRemoved(id));
        }

        public void AckUpdate()
        {
            this.UnseenUpdates.RemoveFirst();
        }

        public Point3Int GetTopHex(int x, int y)
        {
            return this.Terrain.GetTopHex(new Point2Int(x, y));
        }

        public Point3Int GetTopHex(Point2Int location)
        {
            return this.Terrain.GetTopHex(location);
        }
    }
}