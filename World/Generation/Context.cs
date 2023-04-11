namespace Core
{
    public class Context
    {
        private World? world;
        public World World => world ?? throw new System.InvalidOperationException("World is not set");

        public Context()
        {
            this.world = null;
        }

        public Context(World? world)
        {
            this.world = world;
        }

        public void SetWorld(World world)
        {
            this.world = world;
        }
    }
}
