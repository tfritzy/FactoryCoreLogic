namespace Core
{
    public class Sorter : Component
    {
        public override ComponentType Type => ComponentType.Sorter;

        public Sorter(Entity owner) : base(owner)
        {
        }

        public override Schema.Component ToSchema()
        {
            throw new System.NotImplementedException();
        }
    }
}