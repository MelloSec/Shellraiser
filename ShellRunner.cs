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
            if (args.Length != 3)
            {
                Console.WriteLine("Invalid number of arguments. Usage: ShellRunner -b|-u|-r IP_ADDRESS PORT");
                return;
            }

            string option = args[0];
            string ipAddress = args[1];
            int port = int.Parse(args[2]);

            if (option == "-b")
            {
                // call the TCPBind function with the provided IP address and port
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
                Console.WriteLine("Invalid option. Usage: ShellRunner -b|-u|-r IP_ADDRESS PORT");
                return;
            }

            static void TCPBind(string ipAddress, int port)
            {
                // your existing TCPBind code here, using the provided IP address and port
            }

        }


        //
    } 
    }







  