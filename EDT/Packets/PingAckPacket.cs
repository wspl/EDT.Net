using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDT.Packets
{
    public class PingAckPacket : Packet
    {
        public int PingId
        {
            get { return BitConverter.ToInt32(_dgram, BaseHeaderSize); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize); }
        }

        public static int HeaderSize = BaseHeaderSize + 4;

        public PingAckPacket(int connId, int pingId) :
            base(HeaderSize, PacketType.PingPacket, connId)
        {
            PingId = pingId;
        }

        public PingAckPacket(byte[] dgram) : base(dgram) { }
    }
}
