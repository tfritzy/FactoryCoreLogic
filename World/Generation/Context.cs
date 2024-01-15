using System.Threading;

namespace Core
{
    public class Context
    {
        private World? world;
        public World World => world ?? throw new System.InvalidOperationException("World is not set");
        public Connection Connection { get; set; } = null!;

        public enum ContextType
        {
            Client,
            Host,
        }

        public Context(ContextType type = ContextType.Host, IClient? client = null)
        {
            // Needed for some flows. It's invalid to stay in this state, and is remedied by SetWorld().
            world = null;
            client ??= new Client();
            if (type == ContextType.Client)
            {
                Connection = new ClientConnection(this, client);
            }
            else
            {
                Connection = new HostConnection(this, client);
            }
        }

        public void SetWorld(World world)
        {
            this.world = world;
        }
    }
}
