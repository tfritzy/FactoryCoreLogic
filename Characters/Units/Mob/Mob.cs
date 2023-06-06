namespace Core
{
    public abstract class Mob : Unit
    {
        protected Mob(Context context, int alliance) : base(context, alliance)
        {
        }

        public int GetPower()
        {
            int power = 0;

            Life life = GetComponent<Life>();
            if (life != null)
            {
                power += GetComponent<Life>().Health;
            }

            Attack attack = GetComponent<Attack>();
            if (attack != null)
            {
                float dpsPower = (int)(attack.Damage / attack.BaseCooldown);
                dpsPower *= attack.Range / 5f;
                power += (int)dpsPower;
            }

            return power;
        }
    }
}