namespace Core
{
    public class TransferToInventory : Component
    {
        public override ComponentType Type => ComponentType.TransferToInventory;
        public const float PickupMinPercent = .2f;
        public const float PickupMaxPercent = .55f;

        public TransferToInventory(Entity owner) : base(owner)
        {
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.TransferToInventory();
        }

        private float checkCooldown = 0f;
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            checkCooldown -= deltaTime;
            if (checkCooldown > 0)
            {
                return;
            }
            checkCooldown = .05f;

            if (Owner.Conveyor == null)
            {
                return;
            }

            if (Owner.Inventory == null)
            {
                return;
            }

            var curr = Owner.Conveyor.Items.First;
            float totalDist = Owner.Conveyor.GetTotalDistance();
            while (curr != null)
            {
                float percent = curr.Value.ProgressMeters / totalDist;
                if (percent > PickupMinPercent && percent <= PickupMaxPercent)
                {
                    if (Owner.Inventory.CanAddItem(curr.Value.Item))
                    {
                        Owner.Conveyor.Items.Remove(curr);
                        Owner.Inventory.AddItem(curr.Value.Item);
                    }
                }
                curr = curr.Next;
            }
        }
    }
}