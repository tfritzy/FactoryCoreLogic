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
        ItemMoved,
        ItemObjectAdded,
    }

    public abstract class Update
    {
        public abstract UpdateType Type { get; }
    }
}