using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Shellraiser.Modules
{
    public class Socks4Proxy
    {
        private readonly int _bindPort;
        private readonly IPAddress _bindAddress;

        public Socks4Proxy(IPAddress bindAddress = null, int bindPort = 1080)
        {
            _bindPort = bindPort;
            _bindAddress = bindAddress ?? IPAddress.Any;
        }

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public async Task Start()
        {
            var listener = new TcpListener(_bindAddress, _bindPort);
            listener.Start(100);

            while (!_tokenSource.IsCancellationRequested)
            {
                var client = listener.AcceptTcpClient();

                // handle client in new thread
                var thread = new Thread(async () => await HandleClient(client));
                thread.Start();
            }

            listener.Stop();
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }

        private async Task HandleClient(TcpClient client)
        {
            // read data from client
            var stream = client.GetStream();
            var buffer = new byte[4096];
            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            var data = new byte[bytesRead];
            Array.Copy(buffer, data, bytesRead);

            // read the first byte, which is the SOCKS version
            var version = Convert.ToInt32(data[0]);
        }
    }
}
