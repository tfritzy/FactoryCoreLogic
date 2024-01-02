namespace Core
{
    public class MoveCommand : Command
    {
        public const float MaxDistanceToComplete_Sq = .1f * .1f;
        private Point3Float targetPosition;

        public MoveCommand(Point3Float position, Unit owner) : base(owner)
        {
            targetPosition = position;
        }

        public override void CheckIsComplete()
        {
            if ((targetPosition - owner.Location).SquareMagnitude() < MaxDistanceToComplete_Sq)
            {
                IsComplete = true;
            }
        }
    }
}