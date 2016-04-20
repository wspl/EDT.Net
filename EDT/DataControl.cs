using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDT.Packets;

namespace EDT
{
    public class DataControl
    {
        public Connection Conn;

        public short SendSpeed = 10;
        public short ReceiveSpeed = 10;

        public DataControl(Connection conn)
        {
            Conn = conn;
        }

        internal void OnData(DataPacket packet)
        {
            throw new NotImplementedException();
        }

        internal void OnDataAck(DataAckPacket packet)
        {
            throw new NotImplementedException();
        }

        internal void OnDataAck2(DataAck2Packet packet)
        {
            throw new NotImplementedException();
        }
    }
}
