﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDT.Packets
{
    public class DataAckPacket : Packet
    {
        public int AckSequence
        {
            get { return BitConverter.ToInt32(_dgram, BaseHeaderSize); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize); }
        }

        public int DataSequence
        {
            get { return BitConverter.ToInt32(_dgram, BaseHeaderSize + 4); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize + 4); }
        }

        public static int HeaderSize = BaseHeaderSize + 8;

        public static int MaxAckCount = (Config.MaxPacketSize - HeaderSize) / 4;

        public List<int> AckList
        {
            get
            {
                List<int> ackList = new List<int>();
                int ackCount = (Size - HeaderSize) / 4;
                for (int i = 0; i < ackCount; i += 1)
                {
                    int chunkSequence = BitConverter.ToInt32(_dgram, HeaderSize + i * 4);
                    ackList.Add(chunkSequence);
                }
                return ackList;
            }
            set
            {
                for (int i = 0; i < value.Count; i += 1)
                {
                    int chunkSequence = value[i];
                    BitConverter.GetBytes(chunkSequence).CopyTo(_dgram, HeaderSize + i * 4);
                }
                Size = HeaderSize + value.Count * 4;
            }
        }

        public DataAckPacket(int clientId, int ackSequence, int dataSequence, List<int> ackList) :
            base(HeaderSize, PacketType.DataAckPacket, clientId)
        {
            AckSequence = ackSequence;
            DataSequence = dataSequence;
            AckList = ackList;
        }

        public DataAckPacket(byte[] dgram) : base(dgram) { }
    }
}
