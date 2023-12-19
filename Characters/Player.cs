using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Player : Unit
    {
        public override CharacterType Type => CharacterType.Player;
        public WornItems WornItems => this.GetComponent<WornItems>();
        public ActiveItems ActiveItems => this.GetComponent<ActiveItems>();
        private static readonly string name = "Traveler";
        public override string Name => name;

        public Player(Context context, int alliance) : base(context, alliance)
        {

        }

        public override Schema.Entity BuildSchemaObject()
        {
            return new Schema.Player();
        }

        protected override void InitComponents()
        {
            this.SetComponent(new Inventory(this, 5, 8));
            this.SetComponent(new ActiveItems(this, 8, 2));
            this.SetComponent(new WornItems(this, 1, 5));
        }

        public Building? BuidPreviewBuildingFromItem(int itemIndex, Point2Int location)
        {
            Item? item = this.ActiveItems.GetItemAt(itemIndex);
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

            Building newBuilding = (Building)Character.Create(building.Value, this.Context);
            newBuilding.MarkPreview();
            this.Context.World.AddBuilding(newBuilding, location);
            return newBuilding;
        }

        public void MakePreviewBuildingRealFromItem(int itemIndex, Building building)
        {
            Item? item = this.ActiveItems.GetItemAt(itemIndex);
            if (item == null)
            {
                return;
            }

            if (item.Builds != building.Type)
            {
                return;
            }

            if (building.IsPreview == false)
            {
                return;
            }

            this.ActiveItems.DecrementCountOf(itemIndex, 1);
            building.ClearPreview();
        }

        public void PlaceBlockFromItem(int itemIndex, Point3Int location, HexSide subIndex)
        {
            Item? item = this.ActiveItems.GetItemAt(itemIndex);
            if (item == null)
                return;

            Triangle? existingTri = Context.World.Terrain.GetTri(location, subIndex);
            if (existingTri != null)
                return;

            Item.PlacedTriangleMetadata[]? toPlace = item.Places;
            if (toPlace == null)
                return;

            this.ActiveItems.DecrementCountOf(itemIndex, 1);

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