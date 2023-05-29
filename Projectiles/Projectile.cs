using System;
using System.Collections.Generic;

namespace Core
{
    public class Projectile : Entity
    {
        public Point3Float Location { get; private set; }
        public Point3Float Velocity { get; private set; }
        public ProjectileType Type { get; private set; }
        public Action<Character> DealDamage { get; private set; }
        public Func<Character, bool> IsTarget { get; private set; }
        public int HitsRemaining { get; private set; }
        public float ExplosionRadius { get; private set; }
        public PhysicsRequest? PhysicsRequest { get; private set; }
        private HashSet<ulong> hits = new HashSet<ulong>();

        public Projectile(
            Context context,
            ProjectileType projectile,
            Point3Float location,
            Point3Float velocity,
            Action<Character> dealDamage,
            Func<Character, bool> isTarget,
            int maxHits = 1,
            float explosionRadius = 0f) : base(context)
        {
            this.Type = projectile;
            this.Location = location;
            this.Velocity = velocity;
            this.DealDamage = dealDamage;
            this.IsTarget = isTarget;
            this.HitsRemaining = maxHits;
            this.ExplosionRadius = explosionRadius;
        }

        public void OnCollide(Character character)
        {
            if (hits.Contains(character.Id))
                return;
            else
                hits.Add(character.Id);

            if (IsTarget(character) && HitsRemaining > 0)
            {
                HitsRemaining -= 1;

                if (ExplosionRadius > 0f)
                    PhysicsRequest = new SpherePhysicsRequest(Location, ExplosionRadius);
                else
                    DealDamage(character);
            }
        }

        public void OnCollide(Hex hex)
        {
            if (HitsRemaining <= 0)
            {
                return;
            }

            HitsRemaining = 0;

            if (ExplosionRadius > 0f)
                PhysicsRequest = new SpherePhysicsRequest(Location, ExplosionRadius);
        }

        public void Tick(float deltaTime)
        {
            Location += Velocity * deltaTime;
        }

        public void FulfillPhysicsRequest(List<Character> hits)
        {
            if (PhysicsRequest == null)
                return;

            PhysicsRequest = null;
            foreach (Character hit in hits)
            {
                if (IsTarget(hit))
                {
                    DealDamage(hit);
                }
            }
        }
    }
}