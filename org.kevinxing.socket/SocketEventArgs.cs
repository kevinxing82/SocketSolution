using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace org.kevinxing.socket
{
    public class SocketEventArgs : EventArgs
    {
        public SocketEventArgs(ISocket socket, SocketAsyncOperation operation)
        {
            if (socket == null)
            {
                throw new ArgumentNullException("socket");
            }
            Socket = socket;
            Operation = operation;
        }

        public byte[] Data { get; set; }

        public int DataLength { get { return Data == null ? 0 : Data.Length; } }

        public ISocket Socket { get; private set; }

        public SocketAsyncOperation Operation { get; private set; }
    }
}
