namespace Core
{
    public class PickupItem : Command
    {
        public ulong Id { get; private set; }

        public PickupItem(ulong id, Unit owner) : base(owner)
        {
            this.Id = id;
        }

        public override void CheckIsComplete()
        {
            if (!owner.Context.World.ItemObjects.ContainsKey(Id))
            {
                IsComplete = true;
            }
        }

        public void ExecutePickup()
        {
            owner.Context.World.PickupItem(owner.Id, Id);
            IsComplete = true;
        }
    }
}