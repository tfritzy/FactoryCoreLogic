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
        public float BaseRange { get; private set; }
        public float Range { get; private set; }
        public new Character Owner => (Character)base.Owner;

        public const float MeleeRange = .5f;

        public override Schema.Component ToSchema()
        {
            return new Schema.Attack
            {
                BaseDamage = BaseDamage,
                BaseCooldown = BaseCooldown,
                Projectile = Projectile,
            };
        }

        public Attack(
            Character owner,
            float cooldown,
            int damage,
            float range,
            ProjectileType projectile = ProjectileType.Invalid
            ) : base(owner)
        {
            BaseCooldown = cooldown;
            RemainingCooldown = cooldown;
            BaseDamage = damage;
            Damage = damage;
            BaseRange = range;
            Range = range;
            Projectile = projectile;
        }

        public void PerformAttack(Character target)
        {
            if (RemainingCooldown > 0)
            {
                return;
            }

            if (!IsTarget(target))
            {
                return;
            }

            if (Projectile != ProjectileType.Invalid)
            {
                BuildProjectile();
            }
            else
            {
                DealDamage(target);
            }

            RemainingCooldown = BaseCooldown;
        }

        private void DealDamage(Character target)
        {
            target.GetComponent<Life>().Damage(Damage);
        }

        private bool IsTarget(Character target)
        {
            if (!target.HasComponent<Life>())
            {
                return false;
            }

            return target.Alliance != Owner.Alliance;
        }

        private void BuildProjectile()
        {
            this.World.AddProjectile(
                new Projectile(
                    context: this.Owner.Context,
                    projectile: this.Projectile,
                    location: this.Owner.Location + this.Owner.ProjectileSpawnOffset,
                    velocity: new Point3Float(), // TODO: Define some value somewhere.
                    dealDamage: DealDamage,
                    isTarget: IsTarget,
                    maxHits: 1
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