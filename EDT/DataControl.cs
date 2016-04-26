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

        public Stream DataStream = new MemoryStream();

        public DataControl(Connection conn, IPEndPoint target)
        {
            Conn = conn;
            Target = target;

            DataPool = new Dictionary<int, Dictionary<int, DataPacket>>();
        }

        public async Task AutoAck()
        {
            while (true)
            {
                await Task.Delay(1000);
                foreach (var ackList in AckList)
                {
                    if (ackList.Value.Count > 0)
                    {
                        await SendAck(ackList.Key);
                    }
                }
            }
        }

        public async Task SendAck(int dataSequence)
        {
            await SendDataAckPacketAsync(dataSequence, AckList[dataSequence]);
            AckList[dataSequence].Clear();
        }

        public async Task SendDataAsync(Stream dataStream)
        {
            Dictionary<int, DataPacket> dataChunks = new Dictionary<int, DataPacket>();

            int dataSequence = LastDataSequence += 1;
            DataPool.Add(dataSequence, dataChunks);

            byte[] chunk = new byte[DataPacket.MaxChunkSize];

            int offset = 0;

            while (dataStream.CanSeek)
            {
                dataStream.Seek(offset, SeekOrigin.Begin);

                int readSize = await dataStream.ReadAsync(chunk, 0, chunk.Length);
                if (readSize > 0)
                {
                    await SendDataPacketAsync(dataSequence, chunk, offset, chunk.Length);
                    offset += chunk.Length;
                }
            }
        }

        public async Task SendDataPacketAsync(int dataSequence, byte[] chunk, int offset, int size)
        {
            int chunkSequence = LastChunkSequence[dataSequence] += 1;
            DataPacket dataPacket = new DataPacket(ClientId, dataSequence, chunk, chunkSequence, offset, size);
            DataPool[dataSequence].Add(chunkSequence, dataPacket);

            await Conn.SendAsync(dataPacket.Dgram, Target);
        }

        public void OnData(DataPacket packet)
        {
            // create ack pool for new client
            if (!AckList.ContainsKey(packet.ClientId))
            {
                AckList.Add(packet.DataSequence, new List<int>());
            }

            // drop acked packet
            if (AckList[packet.DataSequence].Contains(packet.ChunkSequence)) {
                return;
            }

            // handle ack
            if (AckList[packet.DataSequence].Count <= DataAckPacket.MaxAckCount)
            {
                AckList[packet.DataSequence].Add(packet.ChunkSequence);

                // out of max ack count -> sending ack packet.
                if (AckList[packet.DataSequence].Count == DataAckPacket.MaxAckCount)
                {
                    Task.Run(async () => {
                        await SendAck(packet.DataSequence);
                    });
                }
            }

            // output chunk data
            Task.Run(async () => {
                await DataStream.WriteAsync(packet.Chunk, packet.ChunkOffset, packet.ChunkSize);
            });
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
