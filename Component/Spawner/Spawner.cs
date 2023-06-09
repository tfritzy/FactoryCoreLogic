namespace Core
{
    public class Spawner : Component
    {
        public override ComponentType Type => ComponentType.Spawner;

        public override Schema.Component ToSchema()
        {
            return new Schema.Spawner();
        }

        public Spawner(Entity owner) : base(owner)
        {
        }
    }
}