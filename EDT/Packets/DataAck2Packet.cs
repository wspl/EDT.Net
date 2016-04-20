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

        protected static int HeaderSize = BaseHeaderSize + 4;

        public DataAck2Packet(int clientId, int ackSequance) :
            base(clientId, PacketType.DataAck2Packet)
        {
            AckSequence = ackSequance;
        }

        public DataAck2Packet(byte[] dgram) : base(dgram) { }
    }
}
