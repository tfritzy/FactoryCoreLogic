namespace Core
{
    public class Life : Component
    {
        public override ComponentType Type => ComponentType.Life;
        public int BaseHealth { get; private set; }
        public int Health { get; private set; }
        public bool IsAlive => Health > 0;

        public override Schema.Component ToSchema()
        {
            throw new System.NotImplementedException();
        }

        public Life(Entity owner, int health) : base(owner)
        {
            BaseHealth = health;
            Health = health;
        }

        public void Damage(int damage)
        {
            Health -= damage;
        }
    }
}