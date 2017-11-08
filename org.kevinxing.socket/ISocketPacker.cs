using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.kevinxing.socket
{
    public interface ISocketPacker
    {
        byte[] Pack();

        void UnPack(byte[] bytes);
    }
}
