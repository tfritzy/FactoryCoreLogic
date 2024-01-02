namespace Core
{
    public abstract class Command
    {
        protected Unit owner;
        public abstract void CheckIsComplete();
        public bool IsComplete { get; protected set; }

        public Command(Unit owner)
        {
            this.owner = owner;
        }
    }
}