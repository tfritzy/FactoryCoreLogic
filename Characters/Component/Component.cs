using System;
using System.Collections.Generic;

namespace Core
{
    public abstract class Component : Schema.SerializesTo<Schema.Component>
    {
        public abstract ComponentType Type { get; }
        public Entity Owner { get; set; }
        public virtual void Tick(float deltaTime) { }
        public virtual void OnAddToGrid() { }
        public virtual void OnRemoveFromGrid() { }
        public bool Disabled { get; set; }

        protected World World => Owner.Context.World;

        public Component(Entity owner)
        {
            this.Owner = owner;
        }

        public static readonly Dictionary<Type, ComponentType> ComponentTypeMap = new Dictionary<Type, ComponentType>()
        {
            { typeof(HarvestableComponent), ComponentType.Harvestable },
            { typeof(InventoryComponent), ComponentType.Inventory },
            { typeof(HarvestComponent), ComponentType.Harvest },
            { typeof(ConveyorComponent), ComponentType.Conveyor },
        };

        public abstract Schema.Component ToSchema();
    }
}