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

        public int ChunkOffset
        {
            get { return BitConverter.ToInt32(_dgram, BaseHeaderSize + 8); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize + 8); }
        }

        public int ChunkSize
        {
            get { return BitConverter.ToInt32(_dgram, BaseHeaderSize + 12); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize + 12); }
        }

        public static int HeaderSize = BaseHeaderSize + 16;
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
                          int chunkSequence, int chunkOffset, int chunkSize) :
            base(HeaderSize, PacketType.DataPacket, clientId)
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
