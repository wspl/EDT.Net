using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EDT.Packets;

namespace EDT
{
    class ServerControl
    {
        public Connection Conn;
        public DataControl DataControl;

        public int _clientId;
        public int ClientId
        {
            get
            {
                return _clientId;
            }
            set
            {
                _clientId = value;

                if (DataControl != null)
                {
                    DataControl.ClientId = value;
                }
            }
        }

        public IPEndPoint ClientIPEndPoint;

        public ServerControl(Connection conn, int clientId, IPEndPoint clientIPEndPoint)
        {
            Conn = conn;
            ClientId = clientId;
            ClientIPEndPoint = clientIPEndPoint;

            DataControl = new DataControl(conn, 0, clientIPEndPoint);
        }

        /// <summary>
        /// Handle the ping action sent from client.
        /// </summary>
        /// <param name="pingPacket">Ping packet received from client.</param>
        public void OnPing(PingPacket pingPacket)
        {
            DataControl.Sender.SendSpeed = pingPacket.DownloadSpeed;

            Ping2Packet ping2Packet = new Ping2Packet(ClientId, pingPacket.PingId);

            Conn.BeginSend(ping2Packet.Dgram, ClientIPEndPoint);
        }
    }
}
