using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.kevinxing.socket
{
    public class SocketAsyncState
    {
        public bool Completed { get; set; }

        public byte[] Data { get; set; }

        public bool IsAsync { get; set; }
    }
}
