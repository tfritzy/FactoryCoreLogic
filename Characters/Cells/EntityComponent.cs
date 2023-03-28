using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace FactoryCore
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class EntityComponent
    {
        public EntityComponent(Context context)
        {
            this.Context = context;
            this.Cells = new Dictionary<Type, Cell>();
            this.Id = GenerateId();
            InitCells();
        }

        [JsonProperty("cells")]
        protected Dictionary<Type, Cell> Cells;

        [JsonProperty("id")]
        public ulong Id;

        public Context Context { get; set; }

        public InventoryCell? Inventory => GetCell<InventoryCell>();
        public Harvestable? Harvestable => GetCell<Harvestable>();
        public ConveyorCell? Conveyor => GetCell<ConveyorCell>();

        public bool HasCell<T>() where T : Cell
        {
            return Cells.ContainsKey(typeof(T));
        }

        public T GetCell<T>() where T : Cell
        {
            if (!Cells.ContainsKey(typeof(T)))
            {
                return default(T)!;
            }

            return (T)Cells[typeof(T)];
        }

        public void SetCell(Cell cell)
        {
            Cells[cell.GetType()] = cell;
        }

        protected virtual void InitCells() { }

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
