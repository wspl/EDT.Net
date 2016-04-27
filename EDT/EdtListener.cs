using EDT.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EDT
{
    class EdtListener
    {
        public Connection Conn;

        public Dictionary<int, ServerControl> ServerControls;

        public EdtListener(IPEndPoint localEP)
        {
            Conn = new Connection(ConnectionMode.Server, localEP);

            ServerControls = new Dictionary<int, ServerControl>();
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
                ServerControls[packet.ClientId].DataControl.Receiver.OnData((DataPacket)packet);
            }

            if (packet.Type == PacketType.DataAckPacket)
            {
                ServerControls[packet.ClientId].DataControl.Sender.OnDataAck((DataAckPacket)packet);
            }

            if (packet.Type == PacketType.DataAck2Packet)
            {
                ServerControls[packet.ClientId].DataControl.Receiver.OnDataAck2((DataAck2Packet)packet);
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
                clientId = serverControl.ClientId;

                ServerControls.Add(clientId, serverControl);
                Console.WriteLine("A Client connected in: {0} - {1}", clientId, clientIPEndPoint.ToString());
            }

            ServerControls[clientId].OnPing(pingPacket);
        }
    }
}
