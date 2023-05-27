namespace Core
{
    public class Attack : Component
    {
        public override ComponentType Type => ComponentType.Attack;
        public int BaseDamage { get; private set; }
        public int Damage { get; private set; }
        public float BaseCooldown { get; private set; }
        public float RemainingCooldown { get; private set; }
        public ProjectileType Projectile { get; private set; }

        public override Schema.Component ToSchema()
        {
            return new Schema.Attack
            {
                BaseDamage = BaseDamage,
                BaseCooldown = BaseCooldown,
                Projectile = Projectile,
            };
        }

        public Attack(Entity owner, float cooldown, int damage, ProjectileType projectile) : base(owner)
        {
            BaseCooldown = cooldown;
            RemainingCooldown = cooldown;
            BaseDamage = damage;
            Damage = damage;
            Projectile = projectile;
        }

        public void PerformAttack(Character target)
        {
            if (RemainingCooldown > 0)
            {
                return;
            }

            if (Projectile != ProjectileType.Invalid)
            {
                BuildProjectile();
            }
            else
            {
                target.GetComponent<Life>().Damage(Damage);
            }

            RemainingCooldown = BaseCooldown;
        }

        private void BuildProjectile()
        {
            this.World.AddProjectile(
                new Projectile(
                    this.Owner.Context,
                    this.Projectile,
                    (Character target) => { },
                    (Character target) => false,
                    -1
                )
            );
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            RemainingCooldown -= deltaTime;
        }
    }
}