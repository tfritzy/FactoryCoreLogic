namespace Core
{
    public enum UpdateType
    {
        Invalid,
        Character,
        Location,
        Projectile,
    }

    public abstract class Update
    {
        public abstract UpdateType Type { get; }
    }
}