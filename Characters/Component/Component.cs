namespace Core
{
    public abstract class Component
    {
        public abstract ComponentType Type { get; }

        public Entity Owner { get; set; }
        public virtual void Tick(float deltaTime) { }
        public virtual void OnAddToGrid() { }
        public virtual void OnRemoveFromGrid() { }
        protected World World => Owner.Context.World;

        public Component(Entity owner)
        {
            this.Owner = owner;
        }
    }
}