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

        public Dictionary<int, Dictionary<int, DataPacket>> DataPool = new Dictionary<int, Dictionary<int, DataPacket>>();

        public int LastDataSequence;
        public Dictionary<int, int> LastChunkSequence = new Dictionary<int, int>();

        public Dictionary<int, List<int>> AckList = new Dictionary<int, List<int>>();

        public Dictionary<int, short> DoneAckList = new Dictionary<int, short>(); //null-unsent, 0-sent, 1-done

        public Stream DataStream = new MemoryStream();

        public DataControl(Connection conn, IPEndPoint target)
        {
            Conn = conn;
            Target = target;
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
                        Task.Run(async () => {
                            await SendAck(ackList.Key);
                        });
                    }
                }
            }
        }

        public async Task SendAck(int dataSequence)
        {
            DataAckPacket dataAckPacket = new DataAckPacket(ClientId, 0, dataSequence, AckList[dataSequence]);
            dataAckPacket.AckSequence = dataAckPacket.GetHashCode();
            await Conn.SendAsync(dataAckPacket.Dgram, Target);

            DoneAckList[dataAckPacket.AckSequence] = 0;

            AckList[dataSequence].Clear();

            while (DoneAckList[dataAckPacket.AckSequence] == 1)
            {
                await Task.Delay(1000);
                await Conn.SendAsync(dataAckPacket.Dgram, Target);
            }
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
            // create ack pool for new data
            if (!AckList.ContainsKey(packet.DataSequence))
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

        public void OnDataAck(DataAckPacket packet)
        {
            throw new NotImplementedException();
        }

        public void OnDataAck2(DataAck2Packet packet)
        {
            DoneAckList[packet.AckSequence] = 1;
        }
    }
}
