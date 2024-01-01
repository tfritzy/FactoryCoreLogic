namespace Core
{
    public class WorldApi
    {
        private World world;

        public WorldApi(World world)
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
    }
}