using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Core
{
    public abstract class Entity
    {
        protected Dictionary<Type, Component> Components = new Dictionary<Type, Component>();
        private Dictionary<Type, List<Component>> BaseClassMap = new Dictionary<Type, List<Component>>();
        public ulong Id;
        public Context Context { get; set; }
        public World World => Context.World;
        public List<Entity>? ContainedEntities { get; private set; }
        public Entity? ContainedBy { get; private set; }

        public Inventory? Inventory => GetComponent<Inventory>();
        public Harvestable? Harvestable => GetComponent<Harvestable>();
        public ConveyorComponent? Conveyor => GetComponent<ConveyorComponent>();

        public Entity(Context context)
        {
            this.Context = context;
            this.Components = new Dictionary<Type, Component>();
            this.ContainedEntities = new List<Entity>();
            this.Id = GenerateId();
            InitComponents();
        }

        public bool HasComponent<T>() where T : Component
        {
            return Components.ContainsKey(typeof(T));
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

        protected virtual void InitComponents() { }

        public static ulong GenerateId()
        {
            byte[] bytes = new byte[8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return BitConverter.ToUInt64(bytes, 0);
        }

        public void AddContainedEntity(Entity entity)
        {
            if (this.ContainedEntities == null)
            {
                this.ContainedEntities = new List<Entity>();
            }

            this.ContainedEntities.Add(entity);
            entity.ContainedBy = this;
        }

        public void RemoveContainedEntity(Entity entity)
        {
            if (this.ContainedEntities == null)
            {
                return;
            }

            this.ContainedEntities.Remove(entity);
            entity.ContainedBy = null;
        }

        public virtual void Destroy()
        {
            if (this.ContainedEntities != null)
            {
                for (int i = 0; i < this.ContainedEntities.Count; i++)
                {
                    this.ContainedEntities[i].Destroy();
                }
                this.ContainedEntities = null;
            }

            ContainedBy?.RemoveContainedEntity(this);
            ContainedBy = null;
        }

        public void SetContainedBy(Hex? hex)
        {
            ContainedBy = hex;
        }

        public abstract Schema.Entity BuildSchemaObject();
        public virtual Schema.Entity ToSchema()
        {
            var schema = BuildSchemaObject();
            schema.Id = this.Id;
            schema.ContainedEntities =
                this.ContainedEntities?
                .Select((Entity e) => e.ToSchema())
                .ToList();
            return schema;
        }
    }
}
