namespace Core
{
    public class LocalClient : WorldApi
    {
        private World world;

        public LocalClient(World world)
        {
            this.world = world;
        }

        public void SetUnitLocation(ulong unitId, Point3Float pos)
        {
            Character? c = world.GetCharacter(unitId);
            if (c == null)
            {
                return;
            }

            if (c is Unit unit)
            {
                unit.SetLocation(pos);
            }
        }

        public void PluckBush(ulong pluckerId, Point2Int pos)
        {
            world.PluckBush(pluckerId, pos);
        }

        public void SetItemObjectPos(ulong itemId, Point3Float pos, Point3Float rotation)
        {
            world.SetItemObjectPos(itemId, pos, rotation);
        }

        public void PickupItem(ulong pickerUperId, ulong itemId)
        {
            world.PickupItem(pickerUperId, itemId);
        }
    }
}