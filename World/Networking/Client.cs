using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public class Client : IClient
    {
        private readonly UdpClient client;

        public Client() : base()
        {
            client = new UdpClient();
        }

        public Task<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<UdpReceiveResult>();

            cancellationToken.Register(() =>
            {
                tcs.TrySetCanceled();
                client.Close();
            });

            client.BeginReceive(ar =>
            {
                try
                {
                    if (!tcs.Task.IsCanceled)
                    {
                        IPEndPoint? remoteEP = null;
                        byte[] receivedBytes = client.EndReceive(ar, ref remoteEP);
                        tcs.TrySetResult(new UdpReceiveResult(receivedBytes, remoteEP));
                    }
                }
                catch (Exception ex)
                {
                    if (!tcs.Task.IsCanceled)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }, null);

            return tcs.Task;
        }

        public int Send(byte[] dgram, int bytes, IPEndPoint? endPoint)
        {
            return client.Send(dgram, bytes, endPoint);
        }

        public Task<int> SendAsync(byte[] message, IPEndPoint hostEndPoint)
        {
            return client.SendAsync(message, message.Length, hostEndPoint);
        }
    }
}