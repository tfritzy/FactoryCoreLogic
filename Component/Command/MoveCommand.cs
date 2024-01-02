namespace Core
{
    public class MoveCommand : Command
    {
        public const float MaxDistanceToComplete_Sq = .1f * .1f;
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