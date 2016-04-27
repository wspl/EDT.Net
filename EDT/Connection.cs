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

        public IPEndPoint ServerIPEndPoint;

        public ConnectionMode Mode { get; }

        public Connection(ConnectionMode mode, IPEndPoint serverIPEndPoint)
        {
            Mode = mode;
            ServerIPEndPoint = serverIPEndPoint;

            if (mode == ConnectionMode.Server)
            {
                Conn = new UdpClient(serverIPEndPoint);
            }
            else if (mode == ConnectionMode.Client)
            {
                Conn = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            }
        }

        public async Task<int> SendAsync(byte[] dgram, IPEndPoint targetIPEndPoint)
        {
            return await Conn.SendAsync(dgram, dgram.Length, targetIPEndPoint);
        }
        
        public async Task<UdpReceiveResult> ReceiveAsync()
        {
            return await Conn.ReceiveAsync();
        }

        /*
        public delegate void ReceiveCallback(byte[] dgram, IPEndPoint sourceIPEndPoint);
        private ReceiveCallback _receiveCallback;

        public void BeginReceive(ReceiveCallback receiveCallback)
        {
            _receiveCallback = receiveCallback;
            Conn.BeginReceive(ReceiveCallbackHandle, null);
        }
        
        private void ReceiveCallbackHandle(IAsyncResult result)
        {
            IPEndPoint sourceIPEndPoint = new IPEndPoint(0, 0);
            byte[] dgram = Conn.EndReceive(result, ref sourceIPEndPoint);
            _receiveCallback(dgram, sourceIPEndPoint);
            Conn.BeginReceive(ReceiveCallbackHandle, null);
        }

        public delegate void SendCallback(byte[] dgram);
        private SendCallback _sendCallback;

        public void BeginSend(byte[] dgram, IPEndPoint targetIPEndPoint, SendCallback sendCallback)
        {
            _sendCallback = sendCallback;
            Conn.BeginSend(dgram, dgram.Length, targetIPEndPoint, SendCallbackHandle, dgram);
        }

        public void BeginSend(byte[] dgram, SendCallback sendCallback)
        {
            _sendCallback = sendCallback;
            Conn.BeginSend(dgram, dgram.Length, SendCallbackHandle, dgram);
        }

        public void BeginSend(byte[] dgram, IPEndPoint targetIPEndPoint)
        {
            Conn.BeginSend(dgram, dgram.Length, targetIPEndPoint, (r) => { }, dgram);
        }

        public void BeginSend(byte[] dgram)
        {
            Conn.BeginSend(dgram, dgram.Length, (r) => { }, dgram);
        }
        
        public async Task<int> SendAsync(byte[] dgram)
        {
            return await Conn.SendAsync(dgram, dgram.Length);
        }
        

        public async Task<int> SendAsync(byte[] dgram, IPEndPoint targetIPEndPoint)
        {
            return await Conn.SendAsync(dgram, dgram.Length, targetIPEndPoint);
        }

        
        private void SendCallbackHandle(IAsyncResult result)
        {
            byte[] dgram = (byte[])result.AsyncState;
            _sendCallback(dgram);
        }
        \*/
    }
}
