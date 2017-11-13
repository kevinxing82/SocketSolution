using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace org.kevinxing.socket
{
    public class TCPClient:SocketBase
    {
        public event EventHandler ConnectCompleted;
        public TCPClient():base()
        {
            socketObj = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
              ProtocolType.Tcp);
        }

        public void Connect(IPEndPoint endPoint)
        {
            if (IsConnected)
            {
                throw new InvalidOperationException("已连接至服务器");
            }
            if (endPoint == null)
            {
                throw new ArgumentNullException("endPoint");
            }
            try
            {
                socketObj.Connect(endPoint);
                if (ConnectCompleted != null)
                {
                    ConnectCompleted(this,null);
                }
                Receive();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
