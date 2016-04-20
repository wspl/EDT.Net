using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDT.Packets;
using System.IO;
using System.Net;

namespace EDT
{
    public class DataControl
    {
        public Connection Conn;
        public IPEndPoint Target;

        public int ClientId;

        public short SendSpeed = 10;
        public short ReceiveSpeed = 10;

        public Dictionary<int, Dictionary<int, DataPacket>> DataPool;

        public int LastDataSequence;
        public Dictionary<int, int> LastChunkSequence;

        public Dictionary<int, List<int>> AckList;

        public DataControl(Connection conn, IPEndPoint target)
        {
            Conn = conn;
            Target = target;

            DataPool = new Dictionary<int, Dictionary<int, DataPacket>>();
        }

        public async Task SendDataAsync(Stream dataStream)
        {
            Dictionary<int, DataPacket> dataChunks = new Dictionary<int, DataPacket>();

            int dataSequence = LastDataSequence += 1;
            DataPool.Add(dataSequence, dataChunks);

            byte[] chunk = new byte[DataPacket.MaxChunkSize];

            while (dataStream.Read(chunk, 0, chunk.Length) > 0)
            {
                await SendDataPacketAsync(dataSequence, chunk);
            }

            StreamReader sr = new StreamReader(dataStream);
            
        }

        public async Task SendDataPacketAsync(int dataSequence, byte[] chunk)
        {
            int chunkSequence = LastChunkSequence[dataSequence] += 1;
            DataPacket dataPacket = new DataPacket(ClientId, dataSequence, chunk, chunkSequence, 0, (short)chunk.Length);
            DataPool[dataSequence].Add(chunkSequence, dataPacket);

            await Conn.SendAsync(dataPacket.Dgram, Target);
        }

        public void OnData(DataPacket packet)
        {
            if (!AckList.ContainsKey(packet.ClientId))
            {
                AckList.Add(packet.ClientId, new List<int>());
            }

            if (AckList[packet.ClientId].Contains(packet.ChunkSequence)) {
                return;
            }

            if (AckList[packet.ClientId].Count < DataAckPacket.MaxAckCount)
            {
                AckList[packet.ClientId].Add(packet.ChunkSequence);
            }
            else
            {
                // TODO: Send AckPacket
            }
        }

        public async Task SendDataAckPacketAsync(int dataSequence, List<int> ackList)
        {
            DataAckPacket dataAckPacket = new DataAckPacket(ClientId, dataSequence, ackList);

            await Conn.SendAsync(dataAckPacket.Dgram, Target);
        }

        public void OnDataAck(DataAckPacket packet)
        {
            throw new NotImplementedException();
        }

        public void OnDataAck2(DataAck2Packet packet)
        {
            throw new NotImplementedException();
        }
    }
}
