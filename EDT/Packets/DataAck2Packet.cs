using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDT.Packets
{
    public class DataAck2Packet : Packet
    {
        public int AckSequence
        {
            get { return BitConverter.ToInt32(_dgram, BaseHeaderSize); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize); }
        }

        public static int HeaderSize = BaseHeaderSize + 4;

        public DataAck2Packet(int clientId, int ackSequence) :
            base(HeaderSize, PacketType.DataAck2Packet, clientId)
        {
            AckSequence = ackSequence;
        }

        public DataAck2Packet(byte[] dgram) : base(dgram) { }
    }
}
