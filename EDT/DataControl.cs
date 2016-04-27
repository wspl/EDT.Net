using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDT.Packets;
using System.IO;
using System.Net;

namespace EDT
{
    public class DataControl
    {
        public Connection Conn;
        public IPEndPoint Target;

        public int _clientId;
        public int ClientId
        {
            get
            {
                return _clientId;
            }
            set {
                _clientId = value;

                if (Sender != null && Receiver != null)
                {
                    Sender.ClientId = value;
                    Receiver.ClientId = value;
                }
            }
        }

        public Sender Sender;
        public Receiver Receiver;

        public DataControl(Connection conn, int clientId, IPEndPoint target)
        {
            Conn = conn;
            ClientId = clientId;

            Sender = new Sender(conn, target, clientId);
            Receiver = new Receiver(conn, target, clientId);
        }
    }
}
