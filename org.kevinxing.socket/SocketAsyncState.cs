using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.kevinxing.socket
{
    public class SocketAsyncState
    {
        public Socket workSocket;
        public byte[] SendBuffer { get; set; }

        public const int BufferSize = 1024;

        public byte[] ReceiveBuffer = new byte[BufferSize]; 
    }
}
