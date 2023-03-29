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
            this.Cells = new Dictionary<Type, Component>();
            this.Id = GenerateId();
            InitComponents();
        }

        [JsonProperty("cells")]
        protected Dictionary<Type, Component> Cells;

        [JsonProperty("id")]
        public ulong Id;

        public Context Context { get; set; }

        public InventoryComponent? Inventory => GetCell<InventoryComponent>();
        public HarvestableComponent? Harvestable => GetCell<HarvestableComponent>();
        public ConveyorComponent? Conveyor => GetCell<ConveyorComponent>();

        public bool HasCell<T>() where T : Component
        {
            return Cells.ContainsKey(typeof(T));
        }

        public T GetCell<T>() where T : Component
        {
            if (!Cells.ContainsKey(typeof(T)))
            {
                return default(T)!;
            }

            return (T)Cells[typeof(T)];
        }

        public void SetCell(Component cell)
        {
            Cells[cell.GetType()] = cell;
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
