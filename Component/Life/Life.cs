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
            return new Schema.Life
            {
                BaseHealth = BaseHealth,
                Health = Health,
            };
        }

        public Life(Entity owner, int health) : base(owner)
        {
            BaseHealth = health;
            Health = health;
        }

        public Life(Entity owner, int baseHealth, int remainingHealth) : base(owner)
        {
            BaseHealth = baseHealth;
            Health = remainingHealth;
        }

        public void Damage(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Owner.Destroy();
            }
        }
    }
}