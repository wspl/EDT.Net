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

            Connection server = new Connection(ConnectionMode.Server);
            Connection client = new Connection(ConnectionMode.Client);

            Gateway serverGateway = new Gateway(server);
            Gateway clientGateway = new Gateway(client);

            Console.ReadKey();
        }
    }
}
