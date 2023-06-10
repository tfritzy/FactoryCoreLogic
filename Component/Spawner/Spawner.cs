namespace Core
{
    public class Spawner : Component
    {
        public override ComponentType Type => ComponentType.Spawner;
        public int Range { get; private set; }

        public Spawner(Entity owner, int range) : base(owner)
        {
            Range = range;
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.Spawner()
            {
                Range = Range
            };
        }
    }
}