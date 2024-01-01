namespace Core
{
    public class LocalClient : WorldApi
    {
        private World world;

        public LocalClient(World world)
        {
            this.world = world;
        }

        public void SetUnitLocation(ulong unitId, Point3Float location)
        {
            Character? c = world.GetCharacter(unitId);
            if (c == null)
            {
                return;
            }

            if (c is Unit unit)
            {
                unit.SetLocation(location);
            }
        }

        public void PluckBush(ulong pluckerId, Point2Int pos)
        {
            this.world.PluckBush(pluckerId, pos);
        }
    }
}