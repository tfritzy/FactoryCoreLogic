using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace FactoryCore
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Entity
    {
        public Entity(Context context)
        {
            this.Context = context;
            this.Components = new Dictionary<Type, Component>();
            this.Id = GenerateId();
            InitComponents();
        }

        [JsonProperty("components")]
        protected Dictionary<Type, Component> Components;

        [JsonProperty("id")]
        public ulong Id;

        public Context Context { get; set; }

        public InventoryComponent? Inventory => GetComponent<InventoryComponent>();
        public HarvestableComponent? Harvestable => GetComponent<HarvestableComponent>();
        public ConveyorComponent? Conveyor => GetComponent<ConveyorComponent>();

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

        public void SetComponent(Component component)
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
