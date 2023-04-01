namespace Core
{
    public class Context
    {
        public World World { get; set; }

        public Context()
        {
        }

        public Context(World? world)
        {
            this.World = world;
        }
    }
}
