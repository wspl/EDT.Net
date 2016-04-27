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
    public class Receiver
    {
        public Connection Conn;

        // Sender IPEndPoint
        public IPEndPoint SenderIPEndPoint;

        public int ClientId;

        // Store the AckList of received packet by DataSequence
        // <DataSequence, List<ChunkSequence>>
        public Dictionary<int, List<int>> AckList = new Dictionary<int, List<int>>();

        // Represent the status of each AckPacket by AckSequence: null-unsent, 0-sent, 1-done
        // <AckSequence, StatusCode>
        public Dictionary<int, short> AckStatusList = new Dictionary<int, short>();

        // Output the data stream received from Sender
        public Stream OutputStream;


        public Receiver(Connection conn, IPEndPoint senderIPEndPoint, int clientId)
        {
            Conn = conn;
            SenderIPEndPoint = senderIPEndPoint;
            ClientId = clientId;
        }

        /// <summary>
        /// [Receiver] Automantic send AckList in queue
        /// </summary>
        /// <returns></returns>
        public async Task AutoAck()
        {
            while (true)
            {
                await Task.Delay(1000);
                foreach (var ackList in AckList)
                {
                    if (ackList.Value.Count > 0)
                    {
                        SendAckBackground(ackList.Key);
                    }
                }
            }
        }

        /// <summary>
        /// [Receiver] Send AckList in queue on background
        /// </summary>
        /// <param name="dataSequence"></param>
        public void SendAckBackground(int dataSequence)
        {
            Task.Run(async () => {
                await SendAckAsync(dataSequence);
            });
        }

        /// <summary>
        /// [Receiver] Send AckList in queue
        /// </summary>
        /// <param name="dataSequence"></param>
        /// <returns></returns>
        public async Task SendAckAsync(int dataSequence)
        {
            DataAckPacket dataAckPacket = new DataAckPacket(ClientId, 0, dataSequence, AckList[dataSequence]);
            dataAckPacket.AckSequence = dataAckPacket.GetHashCode();
            await Conn.SendAsync(dataAckPacket.Dgram, SenderIPEndPoint);

            AckStatusList[dataAckPacket.AckSequence] = 0;

            AckList[dataSequence].Clear();

            while (AckStatusList[dataAckPacket.AckSequence] == 1)
            {
                await Task.Delay(1000);
                await Conn.SendAsync(dataAckPacket.Dgram, SenderIPEndPoint);
            }
        }

        /// <summary>
        /// [Receiver] Handle received data from Sender.
        /// </summary>
        /// <param name="packet"></param>
        public void OnData(DataPacket packet)
        {
            // create ack pool for new data
            if (!AckList.ContainsKey(packet.DataSequence))
            {
                AckList.Add(packet.DataSequence, new List<int>());
            }

            // drop acked packet
            if (AckList[packet.DataSequence].Contains(packet.ChunkSequence))
            {
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
                        await SendAckAsync(packet.DataSequence);
                    });
                }
            }

            // output chunk data
            Task.Run(() => {
                Console.Write("Server Received Data Size: " + packet.Chunk.Length);
            });
        }

        /// <summary>
        /// [Receiver] Handle ack2 information from Sender.
        /// </summary>
        /// <param name="packet"></param>
        public void OnDataAck2(DataAck2Packet packet)
        {
            AckStatusList[packet.AckSequence] = 1;
        }
    }
}
