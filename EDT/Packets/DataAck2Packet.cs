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
            base(clientId, PacketType.DataAck2Packet)
        {
            AckSequence = ackSequence;
        }

        public DataAck2Packet(byte[] dgram) : base(dgram) { }
    }
}
