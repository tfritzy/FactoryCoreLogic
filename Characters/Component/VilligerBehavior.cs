namespace Core
{
    public class VilligerBehavior : Component
    {
        public override ComponentType Type => ComponentType.VilligerBehavior;
        public Building? PlaceOfEmployment { get; private set; }

        public override Schema.Component ToSchema()
        {
            throw new System.NotImplementedException();
        }

        public VilligerBehavior(Entity owner) : base(owner)
        {
        }

        public void SetPlaceOfEmployment(Building? building)
        {
            this.PlaceOfEmployment = building;
        }
    }
}