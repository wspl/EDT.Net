using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDT.Packets
{
    public abstract class Packet
    {
        protected byte[] _dgram = new byte[Config.MaxPacketSize];
        public byte[] Dgram
        {
            get
            {
                byte[] dgram = new byte[Size];
                Array.Copy(_dgram, dgram, Size);
                return dgram;
            }
            set
            {
                _dgram = value;
                Size = value.Length;
            }
        }

        public int Size { get; set; }

        public short Version
        {
            get { return BitConverter.ToInt16(_dgram, 0); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, 0); }
        }

        public PacketType Type
        {
            get { return (PacketType)BitConverter.ToInt16(_dgram, 2); }
            set { BitConverter.GetBytes((short)value).CopyTo(_dgram, 2); }
        }

        public int ConnId
        {
            get { return BitConverter.ToInt32(_dgram, 4); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, 4); }
        }

        protected static int BaseHeaderSize = 8;

        public Packet(int size, PacketType packetType)
        {
            Size = size;

            Version = 0x01;
            Type = packetType;
        }

        public Packet(int size, PacketType packetType, int connId) : this(size, packetType)
        {
            ConnId = connId;
        }

        public Packet(PacketType packetType) : this(1500, packetType) { }

        public Packet(PacketType packetType, int connId) : this(1500, packetType, connId) { }

        public Packet(byte[] dgram)
        {
            Dgram = dgram;
            Size = dgram.Length;
        }
    }


    public enum PacketType
    {
        None = 0x00,

        DataPacket = 0x11,
        DataAckPacket = 0x11,

        PingPacket = 0x21,
        PingAckPacket = 0x22,

        CloseConnPacket = 0x41,
        CloseStreamPacket = 0x42
    }
}
