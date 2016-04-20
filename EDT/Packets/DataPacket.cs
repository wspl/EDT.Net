using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDT.Packets
{
    public class DataPacket : Packet
    {
        public int DataSequence
        {
            get { return BitConverter.ToInt32(_dgram, BaseHeaderSize); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize); }
        }

        public int ChunkSequence
        {
            get { return BitConverter.ToInt32(_dgram, BaseHeaderSize + 4); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize + 4); }
        }

        public short ChunkOffset
        {
            get { return BitConverter.ToInt16(_dgram, BaseHeaderSize + 8); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize + 8); }
        }

        public short ChunkSize
        {
            get { return BitConverter.ToInt16(_dgram, BaseHeaderSize + 10); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize + 10); }
        }

        public static int HeaderSize = BaseHeaderSize + 12;
        public static int MaxChunkSize = Config.MaxPacketSize - HeaderSize;

        public byte[] Chunk
        {
            get
            {
                byte[] data = new byte[_dgram.Length - HeaderSize];
                Array.Copy(_dgram, HeaderSize, data, 0, data.Length);
                return data;
            }
            set
            {
                Array.Copy(value, 0, _dgram, HeaderSize, value.Length);
                Size = HeaderSize + value.Length;
            }
        }

        public DataPacket(int clientId, int dataSequence, byte[] chunk,
                          int chunkSequence, short chunkOffset, short chunkSize) :
            base(clientId, PacketType.DataPacket)
        {
            DataSequence = dataSequence;
            ChunkSequence = chunkSequence;
            ChunkOffset = chunkOffset;
            ChunkSize = chunkSize;
            Chunk = chunk;
        }

        public DataPacket(byte[] dgram) : base(dgram) { }
    }
}
