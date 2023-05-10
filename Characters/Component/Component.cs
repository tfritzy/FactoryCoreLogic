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
            { typeof(Harvestable), ComponentType.Harvestable },
            { typeof(Inventory), ComponentType.Inventory },
            { typeof(Harvester), ComponentType.Harvester },
            { typeof(ConveyorComponent), ComponentType.Conveyor },
            { typeof(WornItems), ComponentType.WornItems },
            { typeof(ActiveItems), ComponentType.ActiveItems },
        };

        public abstract Schema.Component ToSchema();
    }
}