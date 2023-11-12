using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core
{
    public abstract class Entity
    {
        protected Dictionary<Type, Component> Components = new Dictionary<Type, Component>();
        private Dictionary<Type, List<Component>> BaseClassMap = new Dictionary<Type, List<Component>>();
        public ulong Id;
        public Context Context { get; set; }
        public World World => Context.World;

        public Inventory? Inventory => GetComponent<Inventory>();
        public ConveyorComponent? Conveyor => GetComponent<ConveyorComponent>();

        public Entity(Context context)
        {
            this.Context = context;
            this.Components = new Dictionary<Type, Component>();
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

        public abstract Schema.Entity BuildSchemaObject();
        public virtual Schema.Entity ToSchema()
        {
            var schema = BuildSchemaObject();
            schema.Id = this.Id;
            schema.Components =
                Components.Count > 0
                    ? Components.Values.Select(c => c.ToSchema()).ToList()
                    : null;
            return schema;
        }

        public abstract void Destroy();
    }
}
