using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.kevinxing.socket
{
    public class SocketHandlerState
    {
        public byte[] Data { get; set; }

        public IAsyncResult AsyncResult { get; set; }

        public Stream Stream { get; set; }

        public AsyncCallback AsyncCallback { get; set; }

        public int DataLength { get; set; }
    }
}
