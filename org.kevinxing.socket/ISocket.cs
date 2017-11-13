using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace org.kevinxing.socket
{
    public interface ISocket
    {
            string SessionID { get; }
            bool IsConnected { get; }

            void Receive();

            void Send(byte[] data);

            void Disconnect();

            void DisconnectAsync();

            event EventHandler OnDisconnectCompleted;

            event EventHandler<byte[]> OnReceiveCompleted;

            event EventHandler OnSendCompleted;
    }
}
