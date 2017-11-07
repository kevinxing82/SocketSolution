using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace org.kevinxing.socket
{
    public interface ISocketHandler
    {
        IAsyncResult BeginReceive(Stream stream, AsyncCallback callback, object state);

        byte[] EndReceive(IAsyncResult ar);

        IAsyncResult BeginSend(byte[] data, int offset, int count, Stream stream,
            AsyncCallback callback, object state);

        bool EndSend(IAsyncResult ar);
    }
}
