namespace Core
{
    public class PluckBushCommand : Command
    {
        public Point2Int Pos { get; private set; }

        public PluckBushCommand(Point2Int vegepos, Unit owner) : base(owner)
        {
            this.Pos = vegepos;
        }

        public override void CheckIsComplete()
        {
            // This action will be completed by the client calling the PluckBush command.
            if (owner.Context.World.Terrain.GetVegetation(Pos) != VegetationType.Bush)
            {
                IsComplete = true;
            }
        }

        public void ExecutePluck()
        {
            owner.Context.World.PluckBush(owner.Id, Pos);
            IsComplete = true;
        }
    }
}