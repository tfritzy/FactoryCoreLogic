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
        public World? ConnectedWorld;
        public readonly static IPEndPoint MatchmakingServerEndPoint =
            new(IPAddress.Parse("20.29.48.111"), 64132);
        private CancellationTokenSource cts = new();

        public Connection(IClient client)
        {
            this.Client = client;
        }

        public abstract Task Connect(Action onConnected, int timeout = DefaultTimeout_ms);
        public abstract void HandleMessage(IPEndPoint endpoint, byte[] message);
        public abstract Task SendPendingMessages();

        public void Update()
        {
            SendPendingMessages();
        }

        public async Task ReadIncomingMessage(float timeout_ms = -1)
        {
            if (timeout_ms > 0)
            {
                cts = new();
                cts.CancelAfter((int)timeout_ms);
            }
            CancellationToken cancellationToken = cts.Token;

            UdpReceiveResult result = await Client.ReceiveAsync(cancellationToken);
            HandleMessage(result.RemoteEndPoint, result.Buffer);
        }
    }
}