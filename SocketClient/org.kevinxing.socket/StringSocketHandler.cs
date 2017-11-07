using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.kevinxing.socket
{
    public class StringSocketHandler : ISocketHandler
    {
        private Dictionary<IAsyncResult, SocketHandlerState> StateSet;
        private List<SocketHandlerState> SendQueue;

        public StringSocketHandler()
        {
            StateSet = new Dictionary<IAsyncResult, SocketHandlerState>();
            SendQueue = new List<SocketHandlerState>();
        }

        public IAsyncResult BeginReceive(Stream stream, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginSend(byte[] data, int offset, int count, Stream stream, AsyncCallback callback, object state)
        {
            if(data==null)
            {
                throw new ArgumentNullException("data");
            }
            if(offset>data.Length||offset<0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if(count<=0||count>data.Length-offset||count>ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if(stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if(callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            if(!stream.CanWrite)
            {
                throw new ArgumentException("stream不支持写入");
            }

            
            return null;
        }

        public byte[] EndReceive(IAsyncResult ar)
        {
            throw new NotImplementedException();
        }

        public bool EndSend(IAsyncResult ar)
        {
            throw new NotImplementedException();
        }
    }
}
