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
        TriUncoveredOrAdded,
        TriHiddenOrDestroyed,
        VegetationChange,
    }

    public abstract class Update
    {
        public abstract UpdateType Type { get; }
    }
}