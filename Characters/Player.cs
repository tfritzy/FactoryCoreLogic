using System;
using System.Collections.Generic;

namespace Core
{
    public class Player : Character
    {
        public override CharacterType Type => CharacterType.Player;
        public WornItems WornItems => this.GetComponent<WornItems>();
        public ActiveItems ActiveItems => this.GetComponent<ActiveItems>();

        public override Schema.Character ToSchema()
        {
            var player = new Schema.Player();
            return this.PopulateSchema(player);
        }

        public Player(Context context) : base(context)
        {

        }

        protected override void InitComponents()
        {
            this.Components = new Dictionary<Type, Component>
            {
                { typeof(Inventory), new Inventory(this, 10, 14) },
                { typeof(ActiveItems), new ActiveItems(this, 10, 3) },
                { typeof(WornItems), new WornItems(this, 1, 5) },
            };
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
    }
}