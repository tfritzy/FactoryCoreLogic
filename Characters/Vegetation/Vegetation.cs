using Schema;

namespace Core
{
    public abstract class Vegetation : Entity
    {
        public abstract VegetationType Type { get; }
        public Hex? ContainedBy { get; private set; }
        public Point2Float PositionOffset { get; set; }

        protected Vegetation(Context context, Point2Float positionOffset) : base(context)
        {
            PositionOffset = positionOffset;
        }


        public void SetContainedBy(Hex? hex)
        {
            ContainedBy = hex;
        }

        public override void Destroy()
        {
            ContainedBy?.RemoveVegetation(this);
            ContainedBy = null;
        }

        public static Vegetation Create(VegetationType type, Context context)
        {
            switch (type)
            {
                case VegetationType.Tree:
                    return new Tree(context);
                default:
                    throw new System.NotImplementedException("Unknown vegetation type. " + type);
            }
        }

        public override Schema.Entity ToSchema()
        {
            var vegetation = (Schema.Vegetation)base.ToSchema();
            vegetation.PositionOffset = PositionOffset;
            return vegetation;
        }
    }
}