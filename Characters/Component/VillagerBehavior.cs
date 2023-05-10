namespace Core
{
    public class VillagerBehavior : Component
    {
        public override ComponentType Type => ComponentType.VillagerBehavior;
        public Worksite? PlaceOfEmployment { get; private set; }

        public override Schema.Component ToSchema()
        {
            throw new System.NotImplementedException();
        }

        public VillagerBehavior(Entity owner) : base(owner)
        {
        }

        public void SetPlaceOfEmployment(Worksite? worksite)
        {
            this.PlaceOfEmployment = worksite;
        }
    }
}