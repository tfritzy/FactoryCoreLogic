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

        public abstract Schema.Component ToSchema();
    }
}