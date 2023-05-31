namespace Core
{
    public abstract class Vegetation : Entity
    {
        public abstract VegetationType Type { get; }
        public Hex? ContainedBy;

        protected Vegetation(Context context) : base(context)
        {
        }

        public Schema.Hex ToSchema()
        {
            throw new System.NotImplementedException();
        }

        public void Destroy()
        {
            throw new System.NotImplementedException();
        }
    }
}