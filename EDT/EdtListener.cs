using EDT.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EDT
{
    class EdtListener
    {
        public Connection Conn;

        public Dictionary<int, EdtClient> EdtClients;
        public EdtClient NewEdtClient;

        public ClientConnectCallback OnClientConnect;

        public EdtListener(IPEndPoint localEP)
        {
            Conn = new Connection(ConnectionMode.Server, localEP);

            EdtClients = new Dictionary<int, EdtClient>();
            Task.Run(async () => {
                while (true)
                {
                    UdpReceiveResult result = await Conn.ReceiveAsync();
                    ServerRoute(result.Buffer, result.RemoteEndPoint);
                }
            });
        }

        /// <summary>
        /// Server side route for bypassing the different packet received from client.
        /// </summary>
        /// <param name="dgram"></param>
        /// <param name="sourceIPEndPoint"></param>
        private void ServerRoute(byte[] dgram, IPEndPoint sourceIPEndPoint)
        {
            Packet packet = Packet.Parse(dgram);

            if (packet.Type == PacketType.PingPacket)
            {
                HandlePingPacket((PingPacket)packet, sourceIPEndPoint);
            }

            if (packet.Type == PacketType.DataPacket)
            {
                EdtClients[packet.ClientId].ServerControl.DataControl.Receiver.OnData((DataPacket)packet);
            }

            if (packet.Type == PacketType.DataAckPacket)
            {
                EdtClients[packet.ClientId].ServerControl.DataControl.Sender.OnDataAck((DataAckPacket)packet);
            }

            if (packet.Type == PacketType.DataAck2Packet)
            {
                EdtClients[packet.ClientId].ServerControl.DataControl.Receiver.OnDataAck2((DataAck2Packet)packet);
            }
        }

        /// <summary>
        /// Handle the ping packet sent from client and create ServerControl for new client.
        /// </summary>
        /// <param name="pingPacket"></param>
        /// <param name="clientIPEndPoint"></param>
        private void HandlePingPacket(PingPacket pingPacket, IPEndPoint clientIPEndPoint)
        {
            int clientId = pingPacket.ClientId;

            // Whether is a new client
            if (clientId == 0)
            {
                ServerControl serverControl = new ServerControl(Conn, clientIPEndPoint);
                EdtClient edtClient = new EdtClient(serverControl);

                clientId = serverControl.ClientId;

                EdtClients.Add(clientId, edtClient);
                Console.WriteLine("A Client connected in: {0} - {1}", clientId, clientIPEndPoint.ToString());

                OnClientConnect?.Invoke(edtClient);
            }

            EdtClients[clientId].ServerControl.OnPing(pingPacket);
        }

        public delegate void ClientConnectCallback(EdtClient client);
    }
}
