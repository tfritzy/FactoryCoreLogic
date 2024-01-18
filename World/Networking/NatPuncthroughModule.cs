using System;
using System.Net;
using System.Text;

namespace Core
{
    public class NatPunchthroughModule
    {
        public bool ConnectionEstablished { get; private set; }
        public IPEndPoint Peer { get; private set; }
        private IClient client;
        private DateTime lastSendTime = DateTime.Now - TimeSpan.FromSeconds(10);
        private Action? onConnectionEstablished;

        public const float TimeBetweenSends = .2f;
        public const string IntroductionMessage = "P2PIntroduction";
        public const string AckMessage = "AckIntroduction";
        public const string HandshakeComplete = "HandshakeComplete";

        public NatPunchthroughModule(IClient client, IPEndPoint peer, Action? onConnectionEstablished = null)
        {
            this.client = client;
            Peer = peer;
            this.onConnectionEstablished = onConnectionEstablished;
        }

        public void Update()
        {
            if (DateTime.Now - lastSendTime < TimeSpan.FromSeconds(TimeBetweenSends))
            {
                return;
            }

            lastSendTime = DateTime.Now;
            byte[] introduction = Encoding.UTF8.GetBytes(IntroductionMessage);
            try
            {
                client.Send(introduction, introduction.Length, Peer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void HandleMessageFromPeer(byte[] message)
        {
            string strMessage = Encoding.UTF8.GetString(message);
            if (strMessage == IntroductionMessage)
            {
                byte[] ack = Encoding.UTF8.GetBytes(AckMessage);
                client.Send(ack, ack.Length, Peer);

                if (ConnectionEstablished)
                {
                    // If they're still unsure things are good, send another handshake complete
                    byte[] handshakeComplete = Encoding.UTF8.GetBytes(HandshakeComplete);
                    client.Send(handshakeComplete, handshakeComplete.Length, Peer);
                }
            }
            else if (strMessage == AckMessage)
            {
                ConnectionEstablished = true;
                byte[] handshakeComplete = Encoding.UTF8.GetBytes(HandshakeComplete);
                client.Send(handshakeComplete, handshakeComplete.Length, Peer);
                onConnectionEstablished?.Invoke();
            }
            else if (strMessage == HandshakeComplete)
            {
                if (!ConnectionEstablished)
                {
                    ConnectionEstablished = true;
                    onConnectionEstablished?.Invoke();
                }
            }
        }

        public bool IsMessageForNatPunchthrough(byte[] message)
        {
            string strMessage = Encoding.UTF8.GetString(message);
            return strMessage == IntroductionMessage ||
                strMessage == AckMessage ||
                strMessage == HandshakeComplete;
        }
    }
}