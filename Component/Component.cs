using System;
using System.Collections.Generic;

namespace Core
{
    public abstract class Component
    {
        public abstract ComponentType Type { get; }
        public Entity Owner { get; set; }
        public virtual void Tick(float deltaTime) { }
        public virtual void OnAddToGrid() { }
        public virtual void OnRemoveFromGrid() { }
        public virtual void OnOwnerRotationChanged(HexSide rotation) { }

        public bool Disabled { get; set; }

        protected World World => Owner.Context.World;

        public Component(Entity owner)
        {
            this.Owner = owner;
        }

        public static readonly Dictionary<Type, ComponentType> ComponentTypeMap = new Dictionary<Type, ComponentType>()
        {
            { typeof(Inventory), ComponentType.Inventory },
            { typeof(ConveyorComponent), ComponentType.Conveyor },
            { typeof(WornItems), ComponentType.WornItems },
            { typeof(ActiveItems), ComponentType.ActiveItems },
            { typeof(Attack), ComponentType.Attack },
            { typeof(Life), ComponentType.Life },
            { typeof(TowerTargeting), ComponentType.TowerTargeting },
        };

        public abstract Schema.Component ToSchema();
    }
}