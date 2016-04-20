using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDT.Packets
{
    public class Ping2Packet : Packet
    {
        public int PingId
        {
            get { return BitConverter.ToInt32(_dgram, BaseHeaderSize); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize); }
        }

        public static int HeaderSize = BaseHeaderSize + 4;

        public Ping2Packet(int clientId, int pingId) :
            base(HeaderSize, PacketType.Ping2Packet, clientId)
        {
            PingId = pingId;
        }

        public Ping2Packet(byte[] dgram) : base(dgram) { }
    }
}
