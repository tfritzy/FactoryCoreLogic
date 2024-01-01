namespace Core
{
    public class PluckBushCommand : Command
    {
        public PluckBushCommand(Point2Int vegepos, Unit owner) : base(owner)
        {

        }

        public override void CheckIsComplete()
        {
            throw new System.NotImplementedException();
        }
    }
}