using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.kevinxing.socket
{
    public interface ISocket
    {
        bool IsConnected { get; }

        void Send(byte[] data);

        void Disconnect();

        event EventHandler<SocketEventArgs> DisconnectCompleted;

        event EventHandler<SocketEventArgs> ReceiveCompleted;

        event EventHandler<SocketEventArgs> SendCompleted;
    }
}
