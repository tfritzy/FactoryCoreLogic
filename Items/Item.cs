public abstract class Item
{
    public abstract ItemType Type { get; }
    public virtual float Width => 0.1f;
}