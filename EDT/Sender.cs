using EDT.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EDT
{
    public class Sender
    {
        public Connection Conn;
        
        // Receiver IPEndPoint
        public IPEndPoint ReceiverIPEndPoint;

        public int ClientId;

        // KBytes/s Value for controlling packet sending speed.
        public short SendSpeed = 10;

        // Store the data sent but not acked yet.
        // <DataSequence, Dictionary<ChunkSequence, DataPacket>>
        public Dictionary<int, Dictionary<int, DataPacket>> DataPool = new Dictionary<int, Dictionary<int, DataPacket>>();

        // Counter of DataSequence and ChunkSequence
        public int LastDataSequence;
        // <DataSequence, LastChunkSequence>
        public Dictionary<int, int> LastChunkSequence = new Dictionary<int, int>();


        public Sender(Connection conn, IPEndPoint receiverIPEndPoint, int clientId)
        {
            Conn = conn;
            ReceiverIPEndPoint = receiverIPEndPoint;
            ClientId = clientId;
        }

        /// <summary>
        /// [Sender] Handle input stream and divide it into DataPackets
        /// </summary>
        /// <param name="dataStream"></param>
        /// <returns></returns>
        public async Task WriteDataAsync(Stream dataStream)
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

        /// <summary>
        /// [Sender] Send DataPacket to Receiver.
        /// </summary>
        /// <param name="dataSequence"></param>
        /// <param name="chunk"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task SendDataPacketAsync(int dataSequence, byte[] chunk, int offset, int size)
        {
            int chunkSequence = LastChunkSequence[dataSequence] += 1;
            DataPacket dataPacket = new DataPacket(ClientId, dataSequence, chunk, chunkSequence, offset, size);
            DataPool[dataSequence].Add(chunkSequence, dataPacket);

            await Conn.SendAsync(dataPacket.Dgram, ReceiverIPEndPoint);
        }

        /// <summary>
        /// [Sender] Handle ack information from Receiver.
        /// </summary>
        /// <param name="packet"></param>
        public void OnDataAck(DataAckPacket packet)
        {
            foreach (var chunkSequence in packet.AckList)
            {
                DataPool[packet.DataSequence].Remove(chunkSequence);
            }

            Task.Run(async () => {
                await SendAck2Async(packet.AckSequence);
            });
        }

        /// <summary>
        /// [Sender] Send Ack2Packet to Receiver.
        /// </summary>
        /// <param name="ackSequence"></param>
        /// <returns></returns>
        public async Task SendAck2Async(int ackSequence)
        {
            DataAck2Packet dataAck2Packet = new DataAck2Packet(ClientId, ackSequence);
            await Conn.SendAsync(dataAck2Packet.Dgram);
        }
    }
}
