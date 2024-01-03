namespace Core
{
    public class MoveCommand : Command
    {
        public const float MaxDistanceToComplete_Sq = Constants.InteractionRange_Sq / 2f;
        public Point3Float TargetPosition { get; private set; }

        public MoveCommand(Point3Float position, Unit owner) : base(owner)
        {
            TargetPosition = position;
        }

        public override void CheckIsComplete()
        {
            if ((TargetPosition - owner.Location).SquareMagnitude() < MaxDistanceToComplete_Sq)
            {
                IsComplete = true;
            }
        }
    }
}