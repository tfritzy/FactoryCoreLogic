using System;
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
        protected Context context;
        protected IClient client;
        public ConnectionState State { get; protected set; } = ConnectionState.Disconnected;
        public const int DefaultTimeout_ms = 10_000;

        // public abstract void UpdateOwnPosition(ulong unitId, Point3Float location, Point3Float velocity);
        // public abstract void SetItemObjectPos(ulong itemId, Point3Float pos, Point3Float rotation);

        public readonly static IPEndPoint MatchmakingServerEndPoint =
            new(IPAddress.Parse("20.29.48.111"), 64132);

        public Connection(Context context, IClient client)
        {
            this.context = context;
            this.client = client;
        }

        public abstract Task Connect(int timeout = DefaultTimeout_ms);
        public abstract void HandleMessage(IPEndPoint endpoint, byte[] message);
        public abstract void SendPendingMessages();

        public void Update()
        {
            SendPendingMessages();
        }


        public async Task ReadIncomingMessage()
        {
            if (State == ConnectionState.Disconnected)
            {
                return;
            }

            UdpReceiveResult result = await client.ReceiveAsync(CancellationToken.None);

            HandleMessage(result.RemoteEndPoint, result.Buffer);
        }
    }
}