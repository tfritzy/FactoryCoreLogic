using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Core
{
    public abstract class Entity
    {
        protected Dictionary<Type, Component> Components;
        public ulong Id;
        public Context Context { get; set; }

        public InventoryComponent? Inventory => GetComponent<InventoryComponent>();
        public HarvestableComponent? Harvestable => GetComponent<HarvestableComponent>();
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
            if (!Components.ContainsKey(typeof(T)))
            {
                return default(T)!;
            }

            return (T)Components[typeof(T)];
        }

        public virtual void SetComponent(Component component)
        {
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
    }
}
