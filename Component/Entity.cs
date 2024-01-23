using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public abstract class Entity
    {
        protected Dictionary<Type, Component> Components = new Dictionary<Type, Component>();
        private Dictionary<Type, List<Component>> BaseClassMap = new Dictionary<Type, List<Component>>();
        public ulong Id;
        public Context Context { get; set; }
        public World World => Context.World;
        public abstract string Name { get; }

        public Inventory? Inventory => GetComponent<Inventory>();
        public ActiveItems? ActiveItems => this.GetComponent<ActiveItems>();
        public ConveyorComponent? Conveyor => GetComponent<ConveyorComponent>();

        public Entity(Schema.Entity entity, Context context)
        {
            this.Context = context;
            this.Components = new Dictionary<Type, Component>();
            this.Id = entity.Id;

            foreach (var schemaComponent in entity.Components)
            {
                Component component = Component.FromSchema(schemaComponent, this);
                SetComponent(component);
            }

            ConfigureComponents();
        }

        public Entity(Context context)
        {
            this.Context = context;
            this.Components = new Dictionary<Type, Component>();
            this.Id = IdGenerator.GenerateId();
            InitComponents();
            ConfigureComponents();
        }

        public bool HasComponent<T>() where T : Component
        {
            return Components.ContainsKey(typeof(T));
        }

        public bool HasComponent(ComponentType type)
        {
            return Components.Values.Any(c => c.Type == type);
        }

        public Component? GetComponent(ComponentType type)
        {
            return Components.Values.FirstOrDefault(c => c.Type == type);
        }

        public T GetComponent<T>() where T : Component
        {
            var type = typeof(T);
            if (!Components.ContainsKey(type))
            {
                if (BaseClassMap.ContainsKey(type) && BaseClassMap[type].Count > 0)
                {
                    return (T)BaseClassMap[type][0];
                }

                return default(T)!;
            }

            return (T)Components[typeof(T)];
        }

        public virtual void SetComponent(Component component)
        {
            var baseType = component.GetType().BaseType;
            if (baseType != null)
            {
                if (!BaseClassMap.ContainsKey(baseType))
                {
                    BaseClassMap[baseType] = new List<Component>();
                }

                BaseClassMap[baseType].Add(component);
            }

            Components[component.GetType()] = component;
        }

        // Initially add components to the entity. Used in flows where they're not set through deserialization.
        protected virtual void InitComponents() { }

        // Sets component properties. Useful for setting up post-serialization.
        public virtual void ConfigureComponents() { }

        public Schema.Entity ToSchema()
        {
            var entity = new Schema.Entity();
            entity.Id = this.Id;

            if (Components.Count > 0)
                entity.Components.AddRange(Components.Values.Select(c => c.ToSchema()));

            return entity;
        }

        public abstract void Destroy();

        public bool CanGiveItem(Item item)
        {
            if (ActiveItems != null)
            {
                if (ActiveItems.CanAddItem(item))
                {
                    return true;
                }
            }

            if (Inventory != null)
            {
                if (Inventory.CanAddItem(item))
                {
                    return true;
                }
            }

            return false;
        }

        public bool GiveItem(Item item)
        {
            if (ActiveItems != null)
            {
                if (ActiveItems.AddItem(item))
                {
                    return true;
                }
            }

            if (Inventory != null)
            {
                if (Inventory.AddItem(item))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
