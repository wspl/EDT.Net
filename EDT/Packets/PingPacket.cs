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
            get { return BitConverter.ToInt16(_dgram, BaseHeaderSize + 6); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize + 6); }
        }

        public short UploadSpeed
        {
            get { return BitConverter.ToInt16(_dgram, BaseHeaderSize + 8); }
            set { BitConverter.GetBytes(value).CopyTo(_dgram, BaseHeaderSize + 8); }
        }

        public static int HeaderSize = BaseHeaderSize + 10;

        public PingPacket(int clientId, int pingId, short downloadSpeed = 0, short uploadSpeed = 0) :
            base(HeaderSize, PacketType.PingPacket, clientId)
        {
            PingId = pingId;
            if (downloadSpeed != 0 || uploadSpeed != 0)
            {
                DownloadSpeed = downloadSpeed;
                UploadSpeed = uploadSpeed;
            }
            else
            {
                Size = BaseHeaderSize + 4;
            }
        }
        
        public PingPacket(byte[] dgram) : base(dgram) { }
    }
}
