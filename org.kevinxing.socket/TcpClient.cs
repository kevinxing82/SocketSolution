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
    public class TcpClient : SocketBase
    {
        public event EventHandler<SocketEventArgs> ConnectCompleted;

        public TcpClient():base()
        {
            socketObj = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);
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
                    if(ConnectCompleted!=null)
                    {
                        ConnectCompleted(this,new SocketEventArgs(this, 
                            SocketAsyncOperation.Connect));
                    }
                    Receive();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
             }
        }
    }
}
