using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace EDT
{
    class UdpState
    {
        public UdpClient Instance;
        public IPEndPoint Listening;
        public UdpState(UdpClient instance, IPEndPoint listening)
        {
            Instance = instance;
            Listening = listening;
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            Connection server = new Connection(ConnectionMode.Server);
            Connection client = new Connection(ConnectionMode.Client);

            client.ReceiveAsync((data, source) => {
                Console.WriteLine("Client Receive: " + data.Length);
            });

            server.ReceiveAsync((data, source) => {
                Console.WriteLine("Server Receive: " + data.Length);
                source.SendAsync(new byte[666], (e) => {
                    Console.WriteLine("Reply");
                });
            });
             
            client.SendAsync(new byte[100], (data) => {
                Console.WriteLine("Sent");
            });

            Console.ReadKey();
        }
    }
}
