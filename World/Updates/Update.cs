namespace Core
{
    public enum UpdateType
    {
        Invalid,
        Character,
        ProjectileAdded,
        ProjectileRemoved,
        BuildingAdded,
        BuildingRemoved,
        HexUncoveredOrAdded,
        HexHiddenOrDestroyed
    }

    public abstract class Update
    {
        public abstract UpdateType Type { get; }
    }
}