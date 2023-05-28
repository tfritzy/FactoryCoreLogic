namespace Core
{
    public class ProjectileUpdate : Update
    {
        public override UpdateType Type => UpdateType.Projectile;
        public ulong Id { get; private set; }
        public ProjectileUpdate(ulong id)
        {
            Id = id;
        }
    }
}