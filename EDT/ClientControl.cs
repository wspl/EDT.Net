using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EDT.Packets;

namespace EDT
{
    public class ClientControl
    {
        public Connection Conn;
        public DataControl DataControl;

        public int ClientId = 0;

        public int PingId = 0;
        public int PingResponsed = 0;
        
        public ClientControl(Connection conn)
        {
            Conn = conn;
            DataControl = new DataControl(conn);
            DataControl.SendSpeed = Config.UploadSpeed;
            DataControl.ReceiveSpeed = Config.DownloadSpeed;

            BeginPing();
        }

        /// <summary>
        /// Start an automantic ping task.
        /// </summary>
        public void BeginPing()
        {
            Task.Run(async () => {
                while (true)
                {
                    PingId += 1;
                    PingPacket pingPacket = new PingPacket(ClientId, PingId, Config.DownloadSpeed, Config.UploadSpeed);
                    await Conn.SendAsync(pingPacket.Dgram);
                    await Task.Delay(1000);
                }
            });
        }

        /// <summary>
        /// Handle the ping2 action sent from server.
        /// </summary>
        /// <param name="ping2Packet"></param>
        public void OnPing2(Ping2Packet ping2Packet)
        {
            PingResponsed += 1;

            // First getting the clientId appointed by server 
            if (ClientId == 0)
            {
                ClientId = ping2Packet.ClientId;
            }

            Console.WriteLine(PingResponsed);
        }
    }
}
