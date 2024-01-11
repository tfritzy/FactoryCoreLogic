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

        public Schema.Component BuildSchemaComponent()
        {
            return new Schema.Component
            {
                Type = Type,
            };
        }

        public static Component FromSchema(Schema.OneofComponent component, Entity owner)
        {
            if (component.Conveyor != null)
                return new ConveyorComponent(component.Conveyor, owner);
            else if (component.Inventory != null)
                return new Inventory(component.Inventory, owner);
            else if (component.Life != null)
                return new Life(component.Life, owner);
            else if (component.WornItems != null)
                return new WornItems(component.WornItems, owner);
            else if (component.ActiveItems != null)
                return new ActiveItems(component.ActiveItems, owner);
            else if (component.Attack != null)
                return new Attack(component.Attack, owner);
            else if (component.TowerTargeting != null)
                return new TowerTargeting(component.TowerTargeting, owner);
            else if (component.Mine != null)
                return new Mine(component.Mine, owner);
            else if (component.ItemPort != null)
                return new ItemPort(component.ItemPort, owner);
            else if (component.Smelt != null)
                return new Smelt(component.Smelt, owner);
            else if (component.Command != null)
                return new CommandComponent(component.Command, owner);
            else
                throw new Exception($"Unknown component type {component}");
        }

        public abstract Schema.OneofComponent ToSchema();
    }
}