using FactoryCore;

namespace FactoryCore
{
    public abstract class Cell
    {
        public Character Owner { get; private set; }
        public abstract void Tick(float deltaTime);
        public abstract CellType Type { get; }
        public virtual void OnAddToGrid() { }
        public virtual void OnRemoveFromGrid() { }
        protected World World => Owner.World;

        public Cell(Character owner)
        {
            this.Owner = owner;
        }
    }
}