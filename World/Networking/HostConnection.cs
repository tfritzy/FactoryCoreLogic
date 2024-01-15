using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Core
{
    public class HostConnection : Connection
    {
        private List<PlayerDetails> connectedPlayers = new();

        public HostConnection(Context context, IClient client) : base(context, client)
        {
        }

        public override async Task Connect(int timeout = DefaultTimeout_ms)
        {
            // Tell matchmaking server I am a host

            // Wait for response from matchmaking server

            // Wait for introduction from client(s)

            // Start whenever I'm ready
        }

        private void SendMessageToAllPlayers(byte[] message)
        {
            foreach (PlayerDetails player in connectedPlayers)
            {
                client.Send(message, message.Length, player.EndPoint);
            }
        }

        public override void HandleMessage(IPEndPoint endpoint, byte[] message)
        {
            Schema.OneofRequest request = Schema.OneofRequest.Parser.ParseFrom(message);
            context.World.Requests.Enqueue(request);
        }

        public override void SendPendingMessages()
        {
            while (context.World.Updates.Count > 0)
            {
                Schema.OneofUpdate update = context.World.Updates.Dequeue();
                byte[] message = update.ToByteArray();
                SendMessageToAllPlayers(message);
            }
        }
    }
}