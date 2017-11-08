using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace org.kevinxing.socket
{
    public class TCPListenerClient:SocketBase
    {
        private TCPListener listener;
        public  TCPListenerClient(TCPListener listener,Socket socket):base()
        {
            this.listener = listener;
            this.socketObj = socket;
            Receive();
        }
    }
}
