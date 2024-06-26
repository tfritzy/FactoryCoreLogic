using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Schema;

namespace Core
{
    public class World
    {
        public Context Context;
        public Terrain Terrain { get; private set; }
        private Dictionary<Point2Int, ulong> Buildings;
        public Dictionary<ulong, Character> Characters { get; private set; }
        public Dictionary<ulong, Projectile> Projectiles { get; private set; }
        public LinkedList<OneofUpdate> UnseenUpdates = new();
        public Queue<OneofUpdate> _updatesOfFrame = new();
        public float OutsideAirTemperature_C = 20f;
        public Dictionary<ulong, ItemObject> ItemObjects = new();
        public float Time;

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
            Context = context;
        }

        public World(Terrain terrain, Context context)
        {
            this.Characters = new Dictionary<ulong, Character>();
            this.Buildings = new Dictionary<Point2Int, ulong>();
            this.Projectiles = new Dictionary<ulong, Projectile>();
            this.Terrain = terrain;
            Context = context;
            context.SetWorld(this);
        }

        public void SetTerrain(Terrain terrain)
        {
            this.Terrain = terrain;
        }


        /// <summary>
        // Tick logic that should happen always, even for clients
        // that don't tick to update world state. Stuff like moving objects
        // based on their velocity.
        /// </summary>
        public void ClientTick(float deltaTime)
        {
            foreach (ItemObject obj in ItemObjects.Values)
            {
                obj.ClientTick(deltaTime);
            }

            foreach (Character c in Characters.Values)
            {
                if (c is Unit unit)
                {
                    unit.ClientTick(deltaTime);
                }
            }
        }

        public void Tick(float deltaTime)
        {
            Time += deltaTime;
            ClientTick(deltaTime);

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
        }

        public Character AddCharacter(Character character)
        {
            AddUpdateForFrame(
                new OneofUpdate
                {
                    CharacterAdded = new CharacterAdded
                    {
                        Character = character.Serialize(),
                    },
                });
            return GetCharacter(character.Id)!;
        }

        public void RemoveCharacter(ulong id)
        {
            if (!this.Characters.ContainsKey(id))
            {
                return;
            }

            AddUpdateForFrame(
                new OneofUpdate
                {
                    CharacterRemoved = new CharacterRemoved
                    {
                        CharacterId = id,
                    },
                });
        }

        public Building AddBuilding(Building building, Point2Int location)
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

            building.GridPosition = GetTopHex(location);
            AddUpdateForFrame(
                new OneofUpdate
                {
                    BuildingAdded = new BuildingAdded
                    {
                        Building = building.Serialize(),
                        GridPosition = location.ToSchema(),
                    },
                });

            building = Context.World.GetBuildingAt(location)!;

            // This should happen only on the host, so not handled inside HandleMessage();
            // Clients should set things like conveyor prev and next entirely through
            // Serialization.
            building.OnAddToGrid(location);

            return building;
        }

        public void RotateBuilding(ulong buildingId, HexSide rotation)
        {
            if (!this.Characters.ContainsKey(buildingId))
            {
                return;
            }

            AddUpdateForFrame(
                new OneofUpdate
                {
                    BuildingRotated = new BuildingRotated
                    {
                        BuildingId = buildingId,
                        Rotation = rotation,
                    },
                });
        }

        public void RemoveBuilding(Point2Int location)
        {
            ulong buildingId = this.Buildings[location];
            Building building = (Building)this.Characters[buildingId];

            // This should happen only on the host, so not handled inside HandleMessage();
            building.OnRemoveFromGrid();

            AddUpdateForFrame(
                new OneofUpdate
                {
                    BuildingRemoved = new BuildingRemoved
                    {
                        BuildingId = buildingId,
                        GridPosition = building.GridPosition.ToSchema(),
                    },
                });
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
            // TODO: Handle this correctly once projectiles have a serialization plan.
            this.Projectiles.Add(projectile.Id, projectile);
            this.UnseenUpdates.AddLast(
                new OneofUpdate
                {
                    ProjectileAdded = new ProjectileAdded() { ProjectileId = projectile.Id }
                });
        }

        public void RemoveProjectile(ulong id)
        {
            // TODO: Handle this correctly once projectiles have a serialization plan.
            this.Projectiles.Remove(id);
            this.UnseenUpdates.AddLast(
                new OneofUpdate
                {
                    ProjectileRemoved = new ProjectileRemoved() { ProjectileId = id }
                });
        }

        public void AckUpdate()
        {
            if (this.UnseenUpdates.Count == 0)
            {
                return;
            }

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

            AddUpdateForFrame(
                new OneofUpdate
                {
                    TerrainObjectChange = new TerrainObjectChange
                    {
                        GridPosition = pos.ToSchema(),
                        NewType = TerrainObjectType.StrippedBush,
                    },
                });
            return true;
        }

        public void AddItemObject(Item item, Point3Float point, Point3Float velocity)
        {
            ItemObject objectForm = new ItemObject(item, point, velocity)
            {
                Position = point,
                Velocity = velocity
            };
            AddUpdateForFrame(
                new OneofUpdate
                {
                    ItemObjectAdded = new ItemObjectAdded
                    {
                        Item = objectForm.ToSchema(),
                    },
                });
        }

        public void RemoveItemObject(ulong itemId)
        {
            AddUpdateForFrame(
                new OneofUpdate
                {
                    ItemObjectRemoved =
                        new ItemObjectRemoved { ItemId = itemId }
                });
        }

        public void SetItemObjectPos(ulong itemId, Point3Float pos, Point3Float velocity)
        {
            ItemObjects.TryGetValue(itemId, out ItemObject? itemObj);
            if (itemObj == null)
            {
                return;
            }

            AddUpdateForFrame(
                new OneofUpdate
                {
                    ItemVelocityChanged = new ItemVelocityChanged
                    {
                        Id = itemId,
                        Position = pos.ToSchema(),
                        Velocity = velocity.ToSchema(),
                    },
                });
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
                SetItemObjectPos(itemId, itemObject.Position + Point3Float.Up * .5f, itemObject.Velocity);
            }
            else
            {
                RemoveItemObject(itemId);
            }
        }

        public ItemObject? GetItem(ulong id)
        {
            if (ItemObjects.TryGetValue(id, out ItemObject? itemObject))
            {
                return itemObject;
            }

            return null;
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

        private void HandleVelocityChange(VelocityChange update)
        {
            Unit? u = (Unit?)GetCharacter(update.PlayerId);

            if (u != null)
            {
                if (!u.Velocity.IsApproximately(Point3Float.FromSchema(update.Velocity)))
                {
                    var velocityChanged = new UnitVelocityChanged
                    {
                        Id = update.PlayerId,
                        Position = update.Position,
                        Velocity = update.Velocity,
                    };
                    AddUpdateForFrame(new OneofUpdate { UnitVelocityChanged = velocityChanged });
                }
            }
        }

        private void PreviewBuilding(ulong playerId, int itemIndex, Point2Int location, HexSide rotation)
        {
            Character? character = GetCharacter(playerId);
            if (character is Player player)
            {
                player.BuidPreviewBuildingFromItem(itemIndex, location, rotation);
            }
        }

        private void MakePreviewBuildingReal(ulong playerId, int itemIndex)
        {
            Character? character = GetCharacter(playerId);
            if (character is Player player)
            {
                player.MakePreviewBuildingRealFromItem(itemIndex);
            }
        }

        private void RotatePreviewBuilding(ulong playerId, HexSide rotation)
        {
            Character? character = GetCharacter(playerId);
            if (character is Player player)
            {
                player.RotatePreviewBuilding(rotation);
            }
        }

        public void HandleRequest(Schema.OneofRequest request)
        {
            if (request.VelocityChange != null)
                HandleVelocityChange(request.VelocityChange);
            else if (request.PickupItem != null)
                PickupItem(request.PickupItem.CharacterId, request.PickupItem.ItemId);
            else if (request.PluckBush != null)
                PluckBush(request.PluckBush.CharacterId, Point2Int.FromSchema(request.PluckBush.GridPosition));
            else if (request.PreviewBuilding != null)
                PreviewBuilding(
                    request.PreviewBuilding.PlayerId,
                    request.PreviewBuilding.ItemIndex,
                    Point2Int.FromSchema(request.PreviewBuilding.Location),
                    request.PreviewBuilding.Rotation);
            else if (request.MakePreviewBuildingReal != null)
                MakePreviewBuildingReal(
                    request.MakePreviewBuildingReal.PlayerId,
                    request.MakePreviewBuildingReal.ItemIndex);
            else if (request.RotatePreviewBuilding != null)
                RotatePreviewBuilding(
                    request.RotatePreviewBuilding.PlayerId,
                    (HexSide)request.RotatePreviewBuilding.Rotation);
            else
                throw new System.NotImplementedException("Unhandled request: " + request);

        }

        public void AddUpdateForFrame(Schema.OneofUpdate update)
        {
            _updatesOfFrame.Enqueue(update);
            HandleUpdate(update);
        }

        public void HandleUpdate(Schema.OneofUpdate update)
        {
            UnseenUpdates.AddLast(update);

            if (update.TerrainObjectChange != null)
            {
                var gridPos = Point2Int.FromSchema(update.TerrainObjectChange.GridPosition);
                Terrain.TerrainObjects[gridPos.x, gridPos.y] = new TerrainObject(update.TerrainObjectChange.NewType);
            }
            else if (update.ProjectileAdded != null)
            {
                throw new System.NotImplementedException("TODO: Handle projectiles");
            }
            else if (update.BuildingAdded != null)
            {
                Building building = (Building)Building.FromSchema(update.BuildingAdded.Building, Context);
                Point2Int location = Point2Int.FromSchema(update.BuildingAdded.GridPosition);
                this.Characters[building.Id] = building;
                this.Buildings.Add(location, building.Id);
            }
            else if (update.TriUncoveredOrAdded != null)
            {
                var triAdded = update.TriUncoveredOrAdded;
                Point3Int p = Point3Int.FromSchema(triAdded.GridPosition);
                HexSide side = (HexSide)triAdded.Side;
                if (Terrain.TerrainData[p.x, p.y, p.z] == null)
                {
                    Terrain.TerrainData[p.x, p.y, p.z] = new Triangle[6];
                }
                Terrain.TerrainData[p.x, p.y, p.z]![(int)side] = triAdded.Tri;
            }
            else if (update.TriHiddenOrDestroyed != null)
            {
                var triDestroyed = update.TriHiddenOrDestroyed;
                Point3Int p = Point3Int.FromSchema(triDestroyed.GridPosition);
                HexSide side = (HexSide)triDestroyed.Side;
                if (Terrain.TerrainData[p.x, p.y, p.z] == null)
                {
                    Terrain.TerrainData[p.x, p.y, p.z] = new Triangle[6];
                }
                Terrain.TerrainData[p.x, p.y, p.z]![(int)side] = null;
            }
            else if (update.CharacterAdded != null)
            {
                Character character = Character.FromSchema(update.CharacterAdded.Character, Context);
                this.Characters[character.Id] = character;
            }
            else if (update.CharacterRemoved != null)
            {
                this.Characters.Remove(update.CharacterRemoved.CharacterId);
            }
            else if (update.BuildingRemoved != null)
            {
                var removedUpdate = update.BuildingRemoved;
                Building building = (Building)this.Characters[removedUpdate.BuildingId];
                this.Buildings.Remove((Point2Int)Point3Int.FromSchema(removedUpdate.GridPosition));
                this.Characters.Remove(removedUpdate.BuildingId);
            }
            else if (update.BuildingRotated != null)
            {
                var rotated = update.BuildingRotated;
                if (Characters.TryGetValue(rotated.BuildingId, out Character? character))
                {
                    if (character is Building building)
                    {
                        building.SetRotation(rotated.Rotation);
                    }
                }
            }
            else if (update.ProjectileRemoved != null)
            {
                throw new System.NotImplementedException("TODO: Handle projectiles");
            }
            else if (update.ItemObjectAdded != null)
            {
                var itemObject = ItemObject.FromSchema(update.ItemObjectAdded.Item);
                ItemObjects.Add(itemObject.Item.Id, itemObject);
            }
            else if (update.ItemObjectRemoved != null)
            {
                ItemObjects.Remove(update.ItemObjectRemoved.ItemId);
            }
            else if (update.UnitVelocityChanged != null)
            {
                var moved = update.UnitVelocityChanged;
                if (Characters.TryGetValue(moved.Id, out Character? character))
                {
                    if (character is Unit unit)
                    {
                        unit.SetLocation(Point3Float.FromSchema(moved.Position));
                        unit.Velocity = Point3Float.FromSchema(moved.Velocity);
                    }
                }
            }
            else if (update.ItemVelocityChanged != null)
            {
                var moved = update.ItemVelocityChanged;
                if (ItemObjects.TryGetValue(moved.Id, out ItemObject? itemObj))
                {
                    itemObj.Position = Point3Float.FromSchema(moved.Position);
                    itemObj.Velocity = Point3Float.FromSchema(moved.Velocity);
                }
            }
            else
            {
                throw new System.NotImplementedException("Unhandled update: " + update);
            }
        }
    }
}