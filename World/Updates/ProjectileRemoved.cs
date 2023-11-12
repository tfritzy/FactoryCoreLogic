namespace Core
{
    public class ProjectileRemoved : Update
    {
        public override UpdateType Type => UpdateType.ProjectileRemoved;
        public ulong Id { get; private set; }
        public ProjectileRemoved(ulong id)
        {
            Id = id;
        }
    }
}