using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EDT.Packets;
using System.Net.Sockets;
using System.IO;

namespace EDT
{
    public enum EdtClientSide
    {
        ServerSide = 0x01,
        ClientSide = 0x02
    }

    public class EdtClient
    {
        private Connection _conn;

        public EdtClientSide Side;

        private ClientControl _clientControl;
        public ServerControl ServerControl;

        public ConnectCallback OnConnect;

        public EdtClient(IPEndPoint remoteEP)
        {
            Side = EdtClientSide.ClientSide;

            _conn = new Connection(ConnectionMode.Client, remoteEP);

            _clientControl = new ClientControl(_conn);
            Task.Run(async () => {
                while (true) {
                    UdpReceiveResult result = await _conn.ReceiveAsync();
                    ClientRoute(result.Buffer, result.RemoteEndPoint);
                }
            });
        }

        internal EdtClient(ServerControl serverControl)
        {
            _conn = serverControl.Conn;
            ServerControl = serverControl;
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
                _clientControl.OnPing2((Ping2Packet)packet);
                // TODO: Callback
            }

            if (packet.Type == PacketType.DataPacket)
            {
                _clientControl.DataControl.Receiver.OnData((DataPacket)packet);
            }

            if (packet.Type == PacketType.DataAckPacket)
            {
                _clientControl.DataControl.Sender.OnDataAck((DataAckPacket)packet);
            }

            if (packet.Type == PacketType.DataAck2Packet)
            {
                _clientControl.DataControl.Receiver.OnDataAck2((DataAck2Packet)packet);
            }
        }

        public delegate void ConnectCallback(Stream stream);
    }
}
