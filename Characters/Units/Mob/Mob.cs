using System;

namespace Core
{
    public abstract class Mob : Unit
    {
        public Mob(Schema.Unit unit, Context context) : base(unit, context)
        {
        }

        protected Mob(Context context, int alliance) : base(context, alliance)
        {
        }

        public float GetPower()
        {
            float power = 0;

            Life life = GetComponent<Life>();
            if (life != null)
            {
                power += GetComponent<Life>().Health;
            }

            Attack attack = GetComponent<Attack>();
            if (attack != null)
            {
                float dpsPower = (int)(attack.Damage / attack.BaseCooldown);
                dpsPower *= 1f + (attack.Range - Attack.MeleeRange) * .2f;
                power += (int)dpsPower;
            }

            return power;
        }
    }
}