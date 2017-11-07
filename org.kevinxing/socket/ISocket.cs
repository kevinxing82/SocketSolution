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

        void SendAsync(byte[] data);

        void Disconnect();

        void DisconnectAsync();

        //event EventHandler<>
    }
}
