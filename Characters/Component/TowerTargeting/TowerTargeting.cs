namespace Core
{
    public class TowerTargeting : Component
    {
        public override ComponentType Type => ComponentType.TowerTargeting;
        public TowerTargetingMode Mode { get; private set; } = TowerTargetingMode.Closest;

        public override Schema.Component ToSchema()
        {
            return new Schema.TowerTargeting
            {
                Mode = Mode,
            };
        }

        public TowerTargeting(Entity owner) : base(owner)
        {
        }

        public void SetMode(TowerTargetingMode mode)
        {
            Mode = mode;
        }
    }
}