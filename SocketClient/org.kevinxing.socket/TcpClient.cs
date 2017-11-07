using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace org.kevinxing.socket
{
    public class TcpClient : ISocket, IDisposable
    {
        private Socket socketObj;
        private Stream streamObj;
        public ISocketHandler handler { get; set; }
        public event EventHandler<SocketEventArgs> ConnectCompleted;
        public event EventHandler<SocketEventArgs> DisconnectCompleted;
        public event EventHandler<SocketEventArgs> ReceiveCompleted;
        public event EventHandler<SocketEventArgs> SendCompleted;

        public TcpClient()
        {
            socketObj = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);
        }
        public bool IsConnected
        {
            get
            {
                return socketObj.Connected;
            }
        }

        public void Connect(IPEndPoint endPoint)
        {
             if(IsConnected)
            {
                throw new InvalidOperationException("已连接至服务器");
            }
             if(endPoint == null)
            {
                throw new ArgumentNullException("endPoint");
            }
             lock(this)
            {
                try
                {
                    socketObj.Connect(endPoint);
                    streamObj = new NetworkStream(socketObj);
                    if(ConnectCompleted!=null)
                    {
                        ConnectCompleted(this,new SocketEventArgs(this, 
                            SocketAsyncOperation.Connect));
                    }
                    SocketAsyncState state = new SocketAsyncState();
                    handler.BeginReceive(streamObj, ReceiveCallback, state);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
             }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            SocketAsyncState state = (SocketAsyncState)ar.AsyncState;
            byte[] data = handler.EndReceive(ar);
            if(data.Length>0)
            {
                if (ReceiveCompleted != null)
                {
                    ReceiveCompleted(this, new SocketEventArgs(this, SocketAsyncOperation.Receive) { Data = data });
                }
            }
            handler.BeginReceive(streamObj, ReceiveCallback,state);
        }

        public void Disconnect()
        {
            if(!IsConnected)
            {
                throw new InvalidOperationException("未连接至服务器");
            }
            lock(this)
            {
                socketObj.Disconnect(true);
            }
        }

        public void Send(byte[] data)
        {
            if(!IsConnected)
            {
                throw new SocketException(10057);
            }
            if(data == null)
            {
                throw new ArgumentNullException("data");
            }
            if(data.Length==0)
            {
                throw new ArgumentException("data的长度不能为0");
            }

            SocketAsyncState state = new SocketAsyncState();
            state.Data = data;
            try
            {
                handler.BeginSend(data, 0, data.Length, streamObj, SendCallback, state);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            SocketAsyncState state = (SocketAsyncState)ar.AsyncState;
            if(handler.EndSend(ar))
            {
                SendCompleted(this, new SocketEventArgs(this, SocketAsyncOperation.Send) {
                    Data = state.Data });
            }
        }

        public void Dispose()
        {
            lock(this)
            {
                if(IsConnected)
                {
                    socketObj.Disconnect(false);
                }
                socketObj.Close();
            }
        }
    }
}
