using System;
using System.Collections.Generic;

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

        public void BuidPreviewBlockFromItem(int itemIndex, Point3Int location, HexSide subIndex)
        {
            Item? item = this.ActiveItems.GetItemAt(itemIndex);
            if (item == null)
            {
                return;
            }

            Triangle? block = item.Places;
            if (block == null)
            {
                return;
            }

            if (this.Context.World.Terrain.GetTri(location, subIndex) != null)
            {
                return;
            }

            Triangle toPlace = new Triangle()
            {
                Type = block.Type,
                SubType = block.SubType,
                IsPreview = true
            };

            this.Context.World.Terrain.SetTriangle(location, toPlace, subIndex);
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

        public void MakePreviewBlockRealFromItem(int itemIndex, Point3Int location, HexSide subIndex)
        {
            Item? item = this.ActiveItems.GetItemAt(itemIndex);
            if (item == null)
                return;

            Triangle? terrainTri = Context.World.Terrain.GetTri(location, subIndex);
            if (terrainTri == null)
                return;

            if (!terrainTri.IsPreview)
                return;

            if (item.Places != terrainTri)
                return;

            this.ActiveItems.DecrementCountOf(itemIndex, 1);
            terrainTri.IsPreview = false;
            Context.World.Terrain.SetTriangle(location, terrainTri, subIndex);
        }
    }
}