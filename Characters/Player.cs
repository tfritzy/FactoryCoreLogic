using System;
using System.Collections.Generic;
using Schema;

namespace Core
{
    public class Player : Unit
    {
        public Guid PlayerId { get; private set; }
        public override CharacterType Type => CharacterType.Player;
        public WornItems WornItems => this.GetComponent<WornItems>();
        private static readonly string name = "Traveler";
        public override string Name => name;
        private Building? previewBuilding;

        public Player(Context context, Schema.Player player) : base(player.Unit, context)
        {
            PlayerId = Guid.Parse(player.PlayerId);
        }

        public Player(Context context, int alliance, Guid playerId) : base(context, alliance)
        {
            PlayerId = playerId;
        }

        public override OneofCharacter Serialize()
        {
            return new OneofCharacter
            {
                Player = new Schema.Player()
                {
                    Unit = base.ToSchema(),
                    PlayerId = PlayerId.ToString()
                }
            };
        }

        protected override void InitComponents()
        {
            this.SetComponent(new Inventory(this, 5, 8));
            this.SetComponent(new ActiveItems(this, 8, 2));
            this.SetComponent(new WornItems(this, 1, 5));
            this.SetComponent(new CommandComponent(this));
        }

        public Building? BuidPreviewBuildingFromItem(int itemIndex, Point2Int location)
        {
            Item? item = ActiveItems?.GetItemAt(itemIndex);
            if (item == null)
            {
                return null;
            }

            CharacterType? building = item.Builds;
            if (building == null)
            {
                return null;
            }

            if (this.Context.World.GetBuildingAt(location) != null)
            {
                return null;
            }

            if (previewBuilding != null && previewBuilding.IsPreview)
            {
                this.Context.World.RemoveBuilding(previewBuilding.Id);
            }

            Building newBuilding = (Building)Character.Create(building.Value, this.Context);
            newBuilding.MarkPreview();
            previewBuilding = this.Context.World.AddBuilding(newBuilding, location);
            return this.Context.World.GetBuildingAt(location);
        }

        public void MakePreviewBuildingRealFromItem(int itemIndex)
        {
            if (previewBuilding == null)
            {
                return;
            }

            if (ActiveItems == null)
            {
                return;
            }

            Item? item = ActiveItems.GetItemAt(itemIndex);
            if (item == null)
            {
                return;
            }

            if (item.Builds != previewBuilding.Type)
            {
                return;
            }

            if (previewBuilding.IsPreview == false)
            {
                return;
            }

            ActiveItems.DecrementCountOf(itemIndex, 1);
            previewBuilding.ClearPreview();
            previewBuilding = null;
        }

        public void PlaceBlockFromItem(int itemIndex, Point3Int location, HexSide subIndex)
        {
            if (ActiveItems == null)
            {
                return;
            }

            Item? item = ActiveItems.GetItemAt(itemIndex);
            if (item == null)
                return;

            Schema.Triangle? existingTri = Context.World.Terrain.GetTri(location, subIndex);
            if (existingTri != null)
                return;

            Item.PlacedTriangleMetadata[]? toPlace = item.Places;
            if (toPlace == null)
                return;

            ActiveItems.DecrementCountOf(itemIndex, 1);

            List<Point3Int> locations = new List<Point3Int>();
            List<HexSide> subIndices = new List<HexSide>();
            foreach (Item.PlacedTriangleMetadata placed in toPlace)
            {
                Point3Int placeLocation = location;
                foreach (HexSide posOffset in placed.PositionOffset)
                {
                    placeLocation = GridHelpers.GetNeighbor(
                        placeLocation,
                        (HexSide)(((int)posOffset + (int)subIndex) % 6));
                }
                var rotatedSubIndex = (HexSide)(((int)placed.RotationOffset + (int)subIndex) % 6);

                if (Context.World.Terrain.GetTri(placeLocation, rotatedSubIndex) != null)
                    return;

                locations.Add(placeLocation);
                subIndices.Add(rotatedSubIndex);
            }

            for (int i = 0; i < locations.Count; i++)
            {
                Point3Int placeLocation = locations[i];
                HexSide rotatedSubIndex = subIndices[i];
                Context.World.Terrain.SetTriangle(placeLocation, toPlace[i].Triangle, rotatedSubIndex);
            }
        }
    }
}