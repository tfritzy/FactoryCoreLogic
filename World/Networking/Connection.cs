using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public abstract class Connection
    {
        public IClient Client { get; private set; }
        public const int DefaultTimeout_ms = 10_000;
        public World? ConnectedWorld { get; private set; }
        public readonly static IPEndPoint MatchmakingServerEndPoint =
            new(IPAddress.Parse("192.168.1.3"), 64132);
        public Guid PlayerId;

        public Connection(IClient client)
        {
            this.Client = client;
            PlayerId = Guid.NewGuid();
        }

        public abstract Task Connect(int timeout = DefaultTimeout_ms);
        public abstract Task HandleMessage(IPEndPoint endpoint, byte[] message);
        public abstract Task SendPendingMessages();

        public void Update()
        {
            SendPendingMessages();
        }

        public virtual void SetWorld(World world)
        {
            ConnectedWorld = world;
        }

        public abstract Task HandleRequest(Schema.OneofRequest request);
    }
}