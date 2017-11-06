using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace SocketClient
{
    class AsyncChatClient
    {    
        public Socket socketObj;
        private StateObject stateObj;
        public delegate void receiveDelegate(string receiveData);//处理接收数据事件的方法类型
        public event receiveDelegate receiveEvent; //接收数据的事件

        public void SendConnection(string ip, int port)  //通过IP地址和端口号发送连接
        {
            IPAddress ipaddr = IPAddress.Parse(ip);//转为IP地址后在连接会加快速度
            IPEndPoint iep = new IPEndPoint(ipaddr, port);
            socketObj = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            stateObj = new StateObject();
            stateObj.workSocket = socketObj;
            try
            {
                socketObj.Connect(iep);
                Console.WriteLine("Socket connected to {0}", socketObj.RemoteEndPoint.ToString());
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            try
            {
                socketObj.BeginReceive(stateObj.buffer, 0, StateObject.BUFFER_SIZE, 0,
                    new AsyncCallback(receiveCallback),stateObj);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Send(string message) //发送信息
        {
            if (socketObj == null)
            {
                return;
            }
            byte[] msg = Encoding.UTF8.GetBytes(message+"\r\n");
            try
            {
                socketObj.BeginSend(msg, 0, msg.Length,SocketFlags.None,
                    new AsyncCallback(sendCallback),stateObj);
            }
            catch (SocketException e)
            {
                Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);
            }
        }

        private void sendCallback(IAsyncResult ar)
        {
            try
            {
                StateObject so = (StateObject)ar.AsyncState;
                int bytesSent = socketObj.EndSend(ar);
                Console.WriteLine("Sent {0} bytes.", bytesSent);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void receiveCallback(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            Socket s = so.workSocket;

            int read = s.EndReceive(ar);

            if(read>0)
            {
                so.sb.Append(Encoding.UTF8.GetString(so.buffer, 0, read));
                s.BeginReceive(so.buffer, 0, StateObject.BUFFER_SIZE, 0,
                                         new AsyncCallback(receiveCallback), so);

                if (so.sb.Length > 1)
                {
                    //All of the data has been read, so displays it to the console
                    string strContent;
                    strContent = so.sb.ToString();
                    Console.WriteLine(String.Format("Read {0} byte from socket" +
                                       "data = {1} ", strContent.Length, strContent));
                    receiveEvent(strContent);//触发事件
                    so.sb.Clear();
                }
            }
        }

        class StateObject
        {
            public Socket workSocket = null;
            public const int BUFFER_SIZE = 1024;
            public byte[] buffer = new byte[BUFFER_SIZE];
            public StringBuilder sb = new StringBuilder();
        }
    }
}
