using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shellraiser
{
    class PortForward
    {
        public static void TCPForward(string ipAddress, int port, string remoteIpAddress, int remotePort)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            listener.Start();
            Console.WriteLine("Listening for incoming connections on " + ipAddress + ":" + port);

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Received a connection from " + client.Client.RemoteEndPoint);

                TcpClient remoteClient = new TcpClient(remoteIpAddress, remotePort);
                Console.WriteLine("Forwarding connection to " + remoteIpAddress + ":" + remotePort);

                NetworkStream stream = client.GetStream();
                NetworkStream remoteStream = remoteClient.GetStream();

                Task.Run(() =>
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        remoteStream.Write(buffer, 0, bytesRead);
                    }
                    remoteClient.Close();
                });

                Task.Run(() =>
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = remoteStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        stream.Write(buffer, 0, bytesRead);
                    }
                    client.Close();
                });
            }
        }
    }
}
