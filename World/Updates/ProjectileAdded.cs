namespace Core
{
    public class ProjectileAdded : Update
    {
        public override UpdateType Type => UpdateType.ProjectileAdded;
        public ulong Id { get; private set; }
        public ProjectileAdded(ulong id)
        {
            Id = id;
        }
    }
}