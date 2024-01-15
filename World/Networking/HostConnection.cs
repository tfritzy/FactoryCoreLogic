using System.Net;
using System.Threading.Tasks;

namespace Core
{
    public class HostConnection : Connection
    {
        public HostConnection(Context context, IClient client)
            : base(context, client, new HostApi(context))
        {
        }

        public override async Task Connect(int timeout = DefaultTimeout_ms)
        {
            // Tell matchmaking server I am a host

            // Wait for response from matchmaking server

            // Wait for introduction from client(s)

            // Start whenever I'm ready
        }

        public override void HandleMessage(IPEndPoint endpoint, byte[] message)
        {
            // Deserialize and forward to context.Api
        }
    }
}