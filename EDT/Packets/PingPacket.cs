using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDT.Packets
{
    public class PingPacket : Packet
    {
        public int PingId
        {
            get { return BitConverter.ToInt32(_dgram, BaseHeaderSize); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize); }
        }

        public short DownloadSpeed
        {
            get { return BitConverter.ToInt16(_dgram, BaseHeaderSize + 4); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize + 4); }
        }

        public short UploadSpeed
        {
            get { return BitConverter.ToInt16(_dgram, BaseHeaderSize + 6); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize + 6); }
        }

        public static int HeaderSize = BaseHeaderSize + 8;

        public PingPacket(int connId, int pingId, short downloadSpeed, short uploadSpeed) :
            base(HeaderSize, PacketType.PingPacket, connId)
        {
            PingId = pingId;
            DownloadSpeed = downloadSpeed;
            UploadSpeed = uploadSpeed;
        }

        public PingPacket(byte[] dgram) : base(dgram) { }
    }
}
