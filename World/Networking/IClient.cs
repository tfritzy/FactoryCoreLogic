using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public interface IClient
    {
        public Task<int> SendAsync(byte[] message, IPEndPoint hostEndPoint);
        public int Send(byte[] dgram, int bytes, IPEndPoint? endPoint);
        public Task<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken);
    }
}