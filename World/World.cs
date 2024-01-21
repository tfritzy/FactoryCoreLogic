using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Schema;

namespace Core
{
    public class World
    {
        public Terrain Terrain { get; private set; }
        private Dictionary<Point2Int, ulong> Buildings;
        private Dictionary<ulong, Character> Characters;
        public Dictionary<ulong, Projectile> Projectiles { get; private set; }
        public LinkedList<Schema.OneofUpdate> UnseenUpdates = new();
        public float OutsideAirTemperature_C = 20f;
        public Dictionary<ulong, ItemObject> ItemObjects = new();
        public Queue<Schema.OneofRequest> Requests = new();
        private Queue<Schema.UpdatePacket> UpdatePackets = new();
        private Queue<Schema.OneofUpdate> UpdatesOfFrame = new();

        public int MaxX => Terrain.MaxX;
        public int MaxY => Terrain.MaxY;
        public int MaxHeight => Terrain.MaxZ;

        public World(Schema.World world, Context context)
        {
            this.Characters = new Dictionary<ulong, Character>();
            this.Buildings = new Dictionary<Point2Int, ulong>();
            this.Projectiles = new Dictionary<ulong, Projectile>();
            this.Terrain = new Terrain(world.Terrain, context);
            foreach (Schema.OneofCharacter schemaCharacter in world.Characters)
            {
                Character character = Character.FromSchema(schemaCharacter, context);
                this.Characters[character.Id] = character;

                if (character is Building building)
                {
                    this.Buildings[(Point2Int)building.GridPosition] = building.Id;
                }
            }

            foreach (Schema.ItemObject schemaItemObject in world.ItemObjects)
            {
                ItemObject itemObject = ItemObject.FromSchema(schemaItemObject);
                this.ItemObjects[itemObject.Item.Id] = itemObject;
            }

            context.SetWorld(this);
        }

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

            CleanupOutOfBoundsItems();
            ChunkUpdatesOfFrame();
        }

        public void AddCharacter(Character character)
        {
            this.Characters[character.Id] = character;
            // this.UnseenUpdates.AddLast(
            //     new OneofUpdate
            //     {
            //         CharacterAdded = new CharacterAdded
            //         {
            //             Character = character.Serialize(),
            //         },
            //     }
            // );
        }

        public void RemoveCharacter(ulong id)
        {
            if (!this.Characters.ContainsKey(id))
            {
                return;
            }

            this.Characters.Remove(id);
            // this.UnseenUpdates.AddLast(
            //     new OneofUpdate
            //     {
            //         CharacterRemoved = new CharacterRemoved
            //         {
            //             CharacterId = id,
            //         },
            //     }
            // );
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

            // this.UnseenUpdates.AddLast(new BuildingAdded((Point2Int)location));
        }

        public void RemoveBuilding(Point2Int location)
        {
            ulong buildingId = this.Buildings[location];
            Building building = (Building)this.Characters[buildingId];
            this.Buildings.Remove(location);
            building.OnRemoveFromGrid();

            // this.UnseenUpdates.AddLast(new BuildingRemoved(buildingId, building.GridPosition));
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

        public byte[] Serialize()
        {
            return ToSchema().ToByteArray();
        }

        public Schema.World ToSchema()
        {
            var characters = Characters.Values
                .Where(c => !c.IsPreview)
                .Select(c => c.Serialize()).ToArray();
            var items = ItemObjects.Values.Select(io => io.ToSchema()).ToArray();
            var world = new Schema.World
            {
                Terrain = Terrain.ToSchema(),
            };
            world.Characters.AddRange(characters);
            world.ItemObjects.AddRange(items);
            return world;
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
            // this.UnseenUpdates.AddLast(new Schema.ProjectileAdded { ProjectileId = projectile.Id });
        }

        public void RemoveProjectile(ulong id)
        {
            this.Projectiles.Remove(id);
            // this.UnseenUpdates.AddLast(new ProjectileRemoved(id));
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

        public Point3Int GetTopHex(int x, int y, HexSide side)
        {
            return this.Terrain.GetTopHex(new Point2Int(x, y), side);
        }

        public bool PluckBush(ulong pluckerId, Point2Int pos)
        {
            if (!Terrain.IsInBounds(pos))
            {
                return false;
            }

            if (Terrain.TerrainObjects[pos.x, pos.y]?.Type != TerrainObjectType.Bush)
            {
                return false;
            }

            Character? plucker = GetCharacter(pluckerId);
            if (plucker == null)
            {
                return false;
            }

            Point3Float bushPos = GridHelpers.EvenRToPixelPlusHeight(GetTopHex(pos));
            float sqDistance = (bushPos - plucker.Location).SquareMagnitude();
            if (sqDistance > Constants.InteractionRange_Sq)
            {
                return false;
            }

            Terrain.TerrainObjects[pos.x, pos.y] = new TerrainObject(TerrainObjectType.StrippedBush);

            var stick = new Stick(1);
            var leaves = new Leaves(1);

            bool sticksGiven = plucker.GiveItem(stick);
            bool leavesGiven = plucker.GiveItem(leaves);

            if (!sticksGiven)
            {
                AddItemObject(stick, bushPos + Point3Float.Up * .5f, Point3Float.Zero);
            }

            if (!leavesGiven)
            {
                AddItemObject(leaves, bushPos + Point3Float.Up * .5f, Point3Float.Zero);
            }

            // UnseenUpdates.AddLast(new TerrainObjectChange(pos, TerrainObjectType.StrippedBush));
            return true;
        }

        public void AddItemObject(Item item, Point3Float point, Point3Float rotation)
        {
            ItemObject objectForm = new ItemObject(item, point, rotation);
            ItemObjects.Add(item.Id, objectForm);
            // UnseenUpdates.AddLast(new ItemObjectAdded(objectForm.ToSchema()));
        }

        public void RemoveItemObject(ulong itemId)
        {
            ItemObjects.Remove(itemId);
            // UnseenUpdates.AddLast(new ItemObjectRemoved(itemId));
        }

        public void SetItemObjectPos(ulong itemId, Point3Float pos, Point3Float rotation)
        {
            ItemObjects.TryGetValue(itemId, out ItemObject? itemObj);
            if (itemObj == null)
            {
                return;
            }

            itemObj.Position = pos;
            itemObj.Rotation = rotation;
            // UnseenUpdates.AddLast(new ItemMoved(itemId, pos, rotation));
        }

        public void PickupItem(ulong pickerUperId, ulong itemId)
        {
            Character? picker = GetCharacter(pickerUperId);
            if (picker == null)
            {
                return;
            }

            if (picker.Inventory == null)
            {
                return;
            }

            ItemObjects.TryGetValue(itemId, out ItemObject? itemObject);
            if (itemObject == null)
            {
                return;
            }

            float sqDistance = (itemObject.Position - picker.Location).SquareMagnitude();
            if (sqDistance > Constants.InteractionRange_Sq)
            {
                return;
            }

            bool fullyGiven = picker.GiveItem(itemObject.Item);
            if (!fullyGiven)
            {
                SetItemObjectPos(itemId, itemObject.Position + Point3Float.Up * .5f, itemObject.Rotation);
            }
            else
            {
                RemoveItemObject(itemId);
            }
        }

        private void CleanupOutOfBoundsItems()
        {
            if (ItemObjects.Count == 0)
            {
                return;
            }

            List<ulong> toRemove = new();
            foreach (var kvp in ItemObjects)
            {
                if (!Terrain.IsInBounds(kvp.Value.Position))
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (ulong id in toRemove)
            {
                RemoveItemObject(id);
            }
        }

        public void SetUnitLocation(ulong unitId, Point3Float pos, Point3Float velocity)
        {
            if (!Characters.ContainsKey(unitId))
            {
                return;
            }

            Character character = Characters[unitId];

            if (character is Unit unit)
            {
                unit.SetLocation(pos);
                // unit.SetVelocity(velocity);
            }
        }

        private void ChunkUpdatesOfFrame()
        {
            if (UpdatesOfFrame.Count == 0)
            {
                return;
            }

            byte[][] updates = UpdatesOfFrame
                .Select(u => u.ToByteArray())
                .ToArray();

            var packets = MessageChunker.Chunk(updates);
            foreach (var packet in packets)
            {
                UpdatePackets.Enqueue(packet);
            }
        }

        public void AddUpdate(Schema.OneofUpdate update)
        {
            UpdatesOfFrame.Enqueue(update);
        }

        public void AddUpdatePacketsToQueue(Schema.UpdatePacket packet)
        {
            UpdatePackets.Enqueue(packet);
        }

        private void HandleUpdates()
        {
            // Unchunk update packets and apply whole updates to the world.
            // Place each update on the unseen updates queue for the frontend.
        }
    }
}