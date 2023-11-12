namespace Core
{
    public class BuildingRemoved : Update
    {
        public override UpdateType Type => UpdateType.BuildingRemoved;
        public ulong Id { get; private set; }

        public BuildingRemoved(ulong id)
        {
            Id = id;
        }
    }
}