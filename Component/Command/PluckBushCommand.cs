namespace Core
{
    public class PluckBushCommand : Command
    {
        private Point2Int pos;

        public PluckBushCommand(Point2Int vegepos, Unit owner) : base(owner)
        {
            this.pos = vegepos;
        }

        public override void CheckIsComplete()
        {
            // This action will be completed by the client calling the PluckBush command.
            if (owner.Context.World.Terrain.GetVegetation(pos) != VegetationType.Bush)
            {
                IsComplete = true;
            }
        }
    }
}