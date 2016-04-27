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
using System.IO;

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
                EdtClient serverGateway = new EdtClient(server);

                string path = @"D:\Plutonist\Pictures\Received.jpeg";
                using (FileStream fs = File.OpenWrite(path))
                {
                    while (serverGateway.ServerControls.Count > 0)
                    {
                        serverGateway.ServerControls.First().Value.DataControl.Receiver.OutputStream = fs;
                    }
                }
            });

            Task.Run(async () => {
                Connection client = new Connection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12344));
                EdtClient clientGateway = new EdtClient(client);

                await Task.Delay(2000);

                string path = @"D:\Plutonist\Pictures\20151111213448_hCFe5.jpeg";
                using (FileStream fs = File.OpenRead(path))
                {
                    await clientGateway._clientControl.DataControl.Sender.WriteDataAsync(fs);
                }
            });

            Console.ReadKey();
        }
    }
}
