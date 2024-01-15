using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
    }

    public abstract class Connection
    {
        private Context context;
        protected IClient client;
        public ConnectionState State { get; protected set; } = ConnectionState.Disconnected;
        public Api Api { get; private set; }
        public const int DefaultTimeout_ms = 10_000;

        public readonly static IPEndPoint MatchmakingServerEndPoint =
            new(IPAddress.Parse("20.29.48.111"), 64132);

        public Connection(Context context, IClient client, Api api)
        {
            this.context = context;
            this.client = client;
            Api = api;
        }

        public abstract Task Connect(int timeout = DefaultTimeout_ms);
        public abstract void HandleMessage(IPEndPoint endpoint, byte[] message);
    }
}