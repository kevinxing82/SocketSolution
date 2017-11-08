using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.kevinxing.socket
{
    public class SocketBase : ISocket, IDisposable
    {
        protected Socket socketObj;
        protected List<byte> receiveCache;
        protected Queue<SocketAsyncState> sendCache;

        public event EventHandler<SocketEventArgs> DisconnectCompleted;
        public event EventHandler<SocketEventArgs> ReceiveCompleted;
        public event EventHandler<SocketEventArgs> SendCompleted;

        public SocketBase()
        {
            receiveCache = new List<byte>();
            sendCache = new Queue<SocketAsyncState>();
        }

        public bool IsConnected
        {
            get
            {
                return socketObj.Connected;
            }
        }

        public void Disconnect()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("未连接至服务器");
            }
            lock (this)
            {
                socketObj.Disconnect(true);
                DisconnectCompleted(this, new SocketEventArgs(this, SocketAsyncOperation.Disconnect));
                socketObj.Close();
            }
        }

        public void DisconnectAsync()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("未连接至服务器");
            }
            lock (this)
            {
                socketObj.BeginDisconnect(true, DisconnectCallback, new object());
            }
        }

        private void DisconnectCallback(IAsyncResult ar)
        {
            socketObj.EndDisconnect(ar);
            socketObj.Close();
            DisconnectCompleted(this, new SocketEventArgs(this, SocketAsyncOperation.Disconnect));
        }

        public void Receive()
        {
            SocketAsyncState state = new SocketAsyncState();
            socketObj.BeginReceive(state.SendBuffer, 0, state.SendBuffer.Length, 0,
                ReceiveCallback, state);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            SocketAsyncState state = (SocketAsyncState)ar.AsyncState;
            int read = socketObj.EndReceive(ar);
            if (read > 0)
            {
                //cach data
                receiveCache.AddRange(state.ReceiveBuffer.Take(read).ToArray());
            }
            else
            {
                if (ReceiveCompleted != null)
                {
                    ReceiveCompleted(this, new SocketEventArgs(this, SocketAsyncOperation.Receive) { Data = receiveCache.ToArray() });
                    receiveCache.Clear();
                }
            }
            socketObj.BeginReceive(state.ReceiveBuffer, 0, state.ReceiveBuffer.Length, 0,
                    ReceiveCallback, state);
        }

        public void Send(byte[] data)
        {
            if (!IsConnected)
            {
                throw new SocketException(10057);
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Length == 0)
            {
                throw new ArgumentException("data的长度不能为0");
            }

            SocketAsyncState state = new SocketAsyncState();
            state.SendBuffer = data;
            if (sendCache.Count > 0)
            {
                sendCache.Enqueue(state);
                SendInternal(sendCache.Dequeue());
            }
            else
            {
                SendInternal(state);
            }
        }

        private void SendInternal(SocketAsyncState state)
        {
            try
            {
                socketObj.BeginSend(state.SendBuffer, 0, state.SendBuffer.Length, 0, SendCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            SocketAsyncState state = (SocketAsyncState)ar.AsyncState;
            int bytesSent = socketObj.EndSend(ar);
            SendCompleted(this, new SocketEventArgs(this, SocketAsyncOperation.Send)
            {
                Data = state.SendBuffer
            });
        }

        public void Dispose()
        {
            
        }
    }
}
