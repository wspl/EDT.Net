using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EDT.Packets;

namespace EDT
{
    class Gateway
    {
        private ClientControl ClientControl;
        private Dictionary<int, ServerControl> ServerControls;

        public Connection Conn;

        public Gateway(Connection conn)
        {
            Conn = conn;

            if (conn.Mode == ConnectionMode.Client)
            {
                Conn.BeginReceive(ClientRoute);
                ClientControl = new ClientControl(conn);
            }

            if (conn.Mode == ConnectionMode.Server)
            {
                ServerControls = new Dictionary<int, ServerControl>();
                Conn.BeginReceive(ServerRoute);
            }
            
        }

        /// <summary>
        /// Client side route for bypassing the different packet received from server.
        /// </summary>
        /// <param name="dgram"></param>
        /// <param name="sourceIPEndPoint"></param>
        private void ClientRoute(byte[] dgram, IPEndPoint sourceIPEndPoint)
        {
            Packet packet = Packet.Parse(dgram);

            if (packet.Type == PacketType.Ping2Packet)
            {
                ClientControl.OnPing2((Ping2Packet)packet);
            }

            if (packet.Type == PacketType.DataPacket)
            {
                ClientControl.DataControl.Receiver.OnData((DataPacket)packet);
            }

            if (packet.Type == PacketType.DataAckPacket)
            {
                ClientControl.DataControl.Sender.OnDataAck((DataAckPacket)packet);
            }

            if (packet.Type == PacketType.DataAck2Packet)
            {
                ClientControl.DataControl.Receiver.OnDataAck2((DataAck2Packet)packet);
            }
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
                ServerControl serverControl = new ServerControl(Conn, 0, clientIPEndPoint);
                clientId = serverControl.GetHashCode();
                serverControl.ClientId = clientId;
                ServerControls.Add(clientId, serverControl);

                Console.WriteLine("A Client connected in: " + clientIPEndPoint.ToString());
            }

            ServerControls[clientId].OnPing(pingPacket);
        }
    }
}
