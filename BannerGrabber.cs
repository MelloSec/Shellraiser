using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Shellraiser
{
    public class BannerGrabber
    {
        public void GrabBanner(string ipAddress, int[] portArr)
        {
            // Loop through the specified ports
            foreach (int port in portArr)
            {
                try
                {
                    // Open a socket connection to the current IP address and port
                    TcpClient client = new TcpClient();
                    client.Connect(ipAddress, port);

                    // Send a GET request to retrieve the service banner
                    NetworkStream stream = client.GetStream();
                    byte[] request = Encoding.ASCII.GetBytes("GET\r\n");
                    stream.Write(request, 0, request.Length);

                    // Read the response from the server
                    byte[] response = new byte[4096];
                    int bytesRead = stream.Read(response, 0, response.Length);
                    string banner = Encoding.ASCII.GetString(response, 0, bytesRead);

                    // Print the banner
                    Console.WriteLine($"Banner for IP {ipAddress} on port {port}: {banner}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occured while grabbing banner for IP {ipAddress} on port {port}: {ex.Message}");
                }
            }
        }
    }
}