using System; // Needed in 4.7.1

namespace FactoryCore
{
    public abstract class Item
    {
        public abstract ItemType Type { get; }
        public virtual float Width => 0.1f;
        public virtual int MaxStack => 1;
        public int Quantity { get; private set; }

        public Item() : this(1) { }

        public Item(int quantity)
        {
            this.Quantity = quantity;
        }

        public void AddToStack(int amount)
        {
            if (Quantity + amount > MaxStack)
                throw new InvalidOperationException("Cannot add to stack, would exceed max stack size.");

            Quantity += amount;
        }

        public void RemoveFromStack(int amount)
        {
            if (Quantity - amount < 0)
                throw new InvalidOperationException("Cannot remove from stack, would go below 0.");

            Quantity -= amount;
        }
    }
}