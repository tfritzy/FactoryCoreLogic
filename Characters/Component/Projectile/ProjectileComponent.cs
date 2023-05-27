using System;

namespace Core
{
    public class ProjectileComponent : Component
    {
        public override ComponentType Type => ComponentType.Projectile;
        public Action<Character> DealDamage { get; private set; }
        public Func<Character, bool> IsTarget { get; private set; }
        public int HitsRemaining { get; private set; }

        public override Schema.Component ToSchema()
        {
            // Projectiles are not serialized.
            throw new System.NotImplementedException();
        }

        public ProjectileComponent(
            Entity owner,
            Action<Character> dealDamage,
            Func<Character, bool> isTarget,
            int hitsRemaining = 1) : base(owner)
        {
            this.DealDamage = dealDamage;
            this.IsTarget = isTarget;
            this.HitsRemaining = hitsRemaining;
        }

        public void OnCollide(Character character)
        {
            if (IsTarget(character) && HitsRemaining > 0)
            {
                DealDamage(character);
                HitsRemaining -= 1;
            }
        }
    }
}