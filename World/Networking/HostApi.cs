namespace Core
{
    public class HostApi : Api
    {
        private Context context;

        public HostApi(Context context) : base(context)
        {
            this.context = context;
        }

        public override void UpdateOwnPosition(ulong unitId, Point3Float pos, Point3Float velocity)
        {
            context.World.SetUnitLocation(unitId, pos, velocity);
        }

        public override void SetItemObjectPos(ulong itemId, Point3Float pos, Point3Float rotation)
        {
            context.World.SetItemObjectPos(itemId, pos, rotation);
        }
    }
}