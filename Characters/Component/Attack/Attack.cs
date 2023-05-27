namespace Core
{
    public class Attack : Component
    {
        public override ComponentType Type => ComponentType.Attack;
        public int BaseDamage { get; private set; }
        public int Damage { get; private set; }
        public float BaseCooldown { get; private set; }
        public float RemainingCooldown { get; private set; }

        public override Schema.Component ToSchema()
        {
            return new Schema.Attack
            {
                BaseDamage = BaseDamage,
                BaseCooldown = BaseCooldown,
            };
        }

        public Attack(Entity owner, float baseCooldown, int baseDamage) : base(owner)
        {
            BaseCooldown = baseCooldown;
            RemainingCooldown = baseCooldown;
            BaseDamage = baseDamage;
            Damage = baseDamage;
        }

        public void PerformAttack(Character target)
        {
            if (RemainingCooldown > 0)
            {
                return;
            }

            if (target.HasComponent<Life>())
            {
                target.GetComponent<Life>().Damage(Damage);
            }

            RemainingCooldown = BaseCooldown;
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            RemainingCooldown -= deltaTime;
        }
    }
}