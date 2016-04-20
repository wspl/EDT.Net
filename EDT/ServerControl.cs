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

        public int ClientId;
        public IPEndPoint ClientIPEndPoint;

        public ServerControl(Connection conn, int clientId, IPEndPoint clientIPEndPoint)
        {
            Conn = conn;
            ClientId = clientId;
            ClientIPEndPoint = clientIPEndPoint;

            DataControl = new DataControl(conn);
        }

        /// <summary>
        /// Handle the ping action sent from client.
        /// </summary>
        /// <param name="pingPacket">Ping packet received from client.</param>
        public void OnPing(PingPacket pingPacket)
        {
            DataControl.SendSpeed = pingPacket.DownloadSpeed;
            DataControl.ReceiveSpeed = pingPacket.UploadSpeed;

            Ping2Packet ping2Packet = new Ping2Packet(ClientId, pingPacket.PingId);

            Conn.BeginSend(ping2Packet.Dgram, ClientIPEndPoint);
        }
    }
}
