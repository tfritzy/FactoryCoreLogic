namespace Core
{
    public class TransferToInventory : Component
    {
        public override ComponentType Type => ComponentType.TransferToInventory;

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
                if (percent > .2f && percent < .5f)
                {

                }
                curr = curr.Next;
            }

            var item = Owner.Inventory.FindItem();
            if (item == null)
            {
                return;
            }

            if (Owner.Conveyor.CanAcceptItem(item))
            {
                Owner.Inventory.RemoveCount(item.Type, 1);
                Owner.Conveyor.AddItem(item);
            }
        }
    }
}