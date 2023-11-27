namespace Core
{
    public class TransferToConveyor : Component
    {
        public override ComponentType Type => ComponentType.TransferToConveyor;

        public TransferToConveyor(Entity owner) : base(owner)
        {
        }

        public override Schema.Component ToSchema()
        {
            return new Schema.TransferToConveyor();
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
            checkCooldown = .1f;

            if (Owner.Conveyor == null)
            {
                return;
            }

            if (Owner.Conveyor.Next == null || Owner.Conveyor.Next.Owner.IsPreview)
            {
                return;
            }

            if (Owner.Inventory == null)
            {
                return;
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