using System;
using System.Collections.Generic;

namespace Core
{
    public class Player : Character
    {
        public override CharacterType Type => CharacterType.Player;

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
    }
}