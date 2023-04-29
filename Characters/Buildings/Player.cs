using System;
using System.Collections.Generic;

namespace Core
{
    public class Player : Character
    {
        public override CharacterType Type => CharacterType.Player;
        public WornItemsComponent WornItems => this.GetComponent<WornItemsComponent>();
        public ActiveItemsComponent ActiveItems => this.GetComponent<ActiveItemsComponent>();

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
                { typeof(InventoryComponent), new InventoryComponent(this, 10, 14) },
                { typeof(ActiveItemsComponent), new ActiveItemsComponent(this, 10, 3) },
                { typeof(WornItemsComponent), new WornItemsComponent(this, 1, 5) },
            };
        }

        public void BuidBuildingFromItem(int itemIndex, Point2Int location)
        {
            Item? item = this.ActiveItems.GetItemAt(itemIndex);
            if (item == null)
            {
                return;
            }

            CharacterType? building = item.Builds;
            if (building == null)
            {
                return;
            }

            if (this.Context.World.GetBuildingAt(location) != null)
            {
                return;
            }

            this.ActiveItems.DecrementCountOf(itemIndex, 1);
            this.Context.World.AddBuilding((Building)Character.Create(building.Value, this.Context), location);
        }
    }
}