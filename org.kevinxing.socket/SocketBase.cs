using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace org.kevinxing.socket
{
    public class SocketBase : ISocket, IDisposable
    {
        private const int ReceiveBufferSize= 1024;
        private string sessionID;
        public Socket socketObj;
        protected byte[] receiveCache;
        protected Queue<byte[]> sendCache;
        protected SocketAsyncEventArgs receiveEventArg;
        protected SocketAsyncEventArgs sendEventArg;

        public event EventHandler OnDisconnectCompleted;
        public event EventHandler<byte[]> OnReceiveCompleted;
        public event EventHandler OnSendCompleted;

        public SocketBase()
        {
            sessionID = Guid.NewGuid().ToString();
            receiveCache = new byte[ReceiveBufferSize];
            sendCache = new Queue<byte[]>();
            receiveEventArg = new SocketAsyncEventArgs();
            receiveEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
            receiveEventArg.SetBuffer(receiveCache, 0, ReceiveBufferSize);
            sendEventArg = new SocketAsyncEventArgs();
            sendEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
        }

        public bool IsConnected
        {
            get
            {
                return socketObj.Connected;
            }
        }

        public string SessionID
        {
            get
            {
                return sessionID;
            }
        }


        private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch(e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                case SocketAsyncOperation.Disconnect:
                    ProcessDisconnect(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was invalid");
            }
        }
        #region Disconnect
        public void Disconnect()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("未连接至服务器");
            }
            try
            {
                socketObj.Disconnect(true);
                receiveEventArg.DisconnectReuseSocket = true;
                ProcessDisconnect(receiveEventArg);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void DisconnectAsync()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("未连接至服务器");
            }
            StartDisconnect(receiveEventArg);
        }

        private void StartDisconnect(SocketAsyncEventArgs e)
        {
            bool willRaiseEvent = socketObj.DisconnectAsync(e);
            if(!willRaiseEvent)
            {
                ProcessDisconnect(e);
            }
        }

        private void ProcessDisconnect(SocketAsyncEventArgs e)
        {
            OnDisconnectCompleted(this, e);
        }
        #endregion

        #region Receive
        public void Receive()
        {
            Console.WriteLine("Receive call");
            StartReceive(receiveEventArg);   
        }

        private void StartReceive(SocketAsyncEventArgs e)
        {
            try
            {
                if (!socketObj.ReceiveAsync(e))
                {
                    ProcessReceive(e);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if(e.BytesTransferred>0&&e.SocketError==SocketError.Success)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(e.Buffer, 0, e.BytesTransferred);
                OnReceiveCompleted(this, buffer.ToArray());
            }
            else
            {
                Console.WriteLine(e.SocketError.ToString());
            }
            Console.WriteLine("ProcessReceive");
            StartReceive(e);
        }
        #endregion

        #region Send
        public void Send(byte[] data)
        {
            if (sendCache.Count > 1)
            {
                sendCache.Enqueue(data);
                StartSend();
            }
            else
            {
                StartSend(data);
            }
        }

        private void StartSend()
        {
            byte[] buffer = sendCache.Dequeue();
            sendEventArg.SetBuffer(buffer, 0, buffer.Length);
            try
            {
                bool willRaiseEvent = socketObj.SendAsync(sendEventArg);
                if(!willRaiseEvent)
                {
                    ProcessSend(sendEventArg);
                }
            }
            catch(Exception e)
            {
                Console.Write(e.ToString());
            }
        }

        private void StartSend(byte[] data)
        {
            sendEventArg.SetBuffer(data, 0, data.Length);
            try
            {
                bool willRaiseEvent = socketObj.SendAsync(sendEventArg);
                if (!willRaiseEvent)
                {
                    ProcessSend(sendEventArg);
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            OnSendCompleted(this,e);
        }
        #endregion

        public void Dispose()
        {
            
        }
    }
}
