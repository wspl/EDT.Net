using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EDT
{
    public enum ConnectionMode
    {
        Server = 0x01,
        Client = 0x02
    }

    public class Connection
    {
        public UdpClient Conn;

        public ConnectionMode Mode { get; }

        public Connection(ConnectionMode mode)
        {
            Mode = mode;

            if (mode == ConnectionMode.Server)
            {
                Conn = new UdpClient(new IPEndPoint(IPAddress.Any, Config.ListenPort));
            }
            else if (mode == ConnectionMode.Client)
            {
                Conn = new UdpClient();
                Conn.Connect(Config.ServerHost, Config.ServerPort);
            }
        }

        public Connection(IPEndPoint clientIPEndPoint)
        {
            Mode = ConnectionMode.Client;
            Conn = new UdpClient();
            Conn.Connect(clientIPEndPoint);
        }

        public delegate void ReceiveCallback(byte[] dgram, Connection source);
        private ReceiveCallback _receiveCallback;

        public void ReceiveAsync(ReceiveCallback receiveCallback)
        {
            _receiveCallback = receiveCallback;
            Conn.BeginReceive(ReceiveCallbackHandle, null);
        }
        
        private void ReceiveCallbackHandle(IAsyncResult result)
        {
            IPEndPoint sourceIPEndPoint = new IPEndPoint(0, 0);
            byte[] dgram = Conn.EndReceive(result, ref sourceIPEndPoint);
            Connection source = new Connection(sourceIPEndPoint);
            _receiveCallback(dgram, source);
            Conn.BeginReceive(ReceiveCallbackHandle, null);
        }

        public delegate void SendCallback(byte[] dgram);
        private SendCallback _sendCallback;

        public void SendAsync(byte[] dgram, SendCallback sendCallback)
        {
            _sendCallback = sendCallback;
            Conn.BeginSend(dgram, dgram.Length, SendCallbackHandle, dgram);
        }

        public void SendAsync(byte[] dgram)
        {
            Conn.BeginSend(dgram, dgram.Length, (r) => { }, dgram);
        }

        private void SendCallbackHandle(IAsyncResult result)
        {
            byte[] dgram = (byte[])result.AsyncState;
            _sendCallback(dgram);
        }
    }
}
