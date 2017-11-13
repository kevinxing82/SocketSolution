using System;
using System.Net;
using System.Text;
using org.kevinxing.socket;
namespace SocketClient
{
    class AsyncChatClient
    {
        public TCPClient client;

        public delegate void receiveDelegate(string receiveData);//处理接收数据事件的方法类型
        public event receiveDelegate receiveEvent; //接收数据的事件

        public void SendConnection(string ip, int port)
        {
            IPAddress ipaddr = IPAddress.Parse(ip);//转为IP地址后在连接会加快速度
            IPEndPoint iep = new IPEndPoint(ipaddr, port);
            client = new TCPClient();

            client.ConnectCompleted += new EventHandler(OnConnected);
            client.OnDisconnectCompleted += new EventHandler(OnDisconnected);
            client.OnSendCompleted += new EventHandler(OnSend);
            client.OnReceiveCompleted += new EventHandler<byte[]>(OnReceived);
            client.Connect(iep);

        }

        private void OnConnected(object sender, EventArgs e)
        {
            Console.WriteLine("Socket connected to {0}", client.socketObj.RemoteEndPoint.ToString());
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Socket disconnect to {0}", client.socketObj.RemoteEndPoint.ToString());
        }

        private void OnSend(object sender, EventArgs e)
        {
            Console.WriteLine("Socket send success");
        }

        private void OnReceived(object sender, byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Encoding.UTF8.GetString(data, 0, data.Length));
            receiveEvent(sb.ToString());
        }

        public void Send(string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message + "\r\n");
            client.Send(msg);
        }
    }
}
