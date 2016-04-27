using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using EDT.Packets;
using System.Diagnostics;

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

            Task.Run(() => {
                Connection server = new Connection(ConnectionMode.Server, new IPEndPoint(IPAddress.Any, 12344));
                Gateway serverGateway = new Gateway(server);
            });

            Task.Run(() => {
                Connection client = new Connection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12344));
                Gateway clientGateway = new Gateway(client);
            });

            Console.ReadKey();
        }
    }
}
