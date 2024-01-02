using System.Threading;

namespace Core
{
    public class Context
    {
        private World? world;
        public World World => world ?? throw new System.InvalidOperationException("World is not set");
        public LocalClient Api;

        public Context()
        {
            // Needed for some flows. It's invalid to stay in this state, and is remedied by SetWorld().
            world = null;
            Api = new LocalClient(null!);
        }

        public Context(World? world)
        {
            this.world = world;
            Api = new LocalClient(World);
        }

        public void SetWorld(World world)
        {
            this.world = world;
            Api = new LocalClient(world);
        }
    }
}
