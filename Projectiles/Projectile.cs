using System;

namespace Core
{
    public class Projectile : Entity
    {
        public ProjectileType Type { get; private set; }
        public Action<Character> DealDamage { get; private set; }
        public Func<Character, bool> IsTarget { get; private set; }
        public int HitsRemaining { get; private set; }

        public Projectile(
            Context context,
            ProjectileType projectile,
            Action<Character> dealDamage,
            Func<Character, bool> isTarget,
            int maxHits = 1) : base(context)
        {
            this.Type = projectile;
            this.DealDamage = dealDamage;
            this.IsTarget = isTarget;
            this.HitsRemaining = maxHits;
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