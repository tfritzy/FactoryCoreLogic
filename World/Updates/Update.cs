namespace Core
{
    public enum UpdateType
    {
        Invalid,
        Character,
        Projectile,
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