namespace Core
{
    public class VillagerBehavior : Component
    {
        public override ComponentType Type => ComponentType.VillagerBehavior;
        public ulong? BuildingOfEmployment { get; private set; }
        public Worksite? PlaceOfEmployment =>
            BuildingOfEmployment != null
                ? this.World.GetCharacter(BuildingOfEmployment.Value)?.GetComponent<Worksite>()
                : null;


        public override Schema.Component ToSchema()
        {
            return new Schema.VillagerBehavior()
            {
                BuildingEmployedAt = this.BuildingOfEmployment,
            };
        }

        public VillagerBehavior(Entity owner) : base(owner)
        {
        }

        public void SetPlaceOfEmployment(ulong? buildingId)
        {
            this.BuildingOfEmployment = buildingId;
        }
    }
}