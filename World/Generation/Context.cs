using System.Threading;

namespace Core
{
    public class Context
    {
        private World? world;
        public World World => world ?? throw new System.InvalidOperationException("World is not set");

        public enum ContextType
        {
            Local,
            Client,
            Host,
        }

        public Context()
        {
            // Needed for some flows. It's invalid to stay in this state, and is remedied by SetWorld().
            world = null;
        }

        public void SetWorld(World world)
        {
            this.world = world;
        }
    }
}
