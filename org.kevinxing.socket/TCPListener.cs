using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.kevinxing.socket
{
    public class TCPListener : IDisposable
    {
        private Socket socket;
        private HashSet<TCPListenerClient> clients;
        private int port;
        public int Port
        {
            get { return port; }
            set
            {
                if (value < 0 || value > 65535)
                {
                    throw new ArgumentOutOfRangeException(value + "不是有效端口。");
                }
                port = value;
            }
        }

        public event EventHandler<SocketEventArgs> AcceptCompleted;
        public event EventHandler<SocketEventArgs> DisconnectCompleted;
        public event EventHandler<SocketEventArgs> ReceiveCompleted;
        public event EventHandler<SocketEventArgs> SendCompleted;

        public void TcpListener()
        {
            clients = new HashSet<TCPListenerClient>();
        }

        public bool IsStarted { get; private set; }
        public void Start(string ip,int port)
        {
            if(IsStarted)
            {
                throw new InvalidOperationException("已经开始服务");
            }
            IPAddress ipaddr = IPAddress.Parse(ip);//转为IP地址后在连接会加快速度
            IPEndPoint iep = new IPEndPoint(ipaddr, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp);
            socket.Bind(iep);
            socket.Listen(512);
            IsStarted = true;
            socket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket clientSocket = socket.EndAccept(ar);
            TCPListenerClient client = new TCPListenerClient(this, clientSocket);
            client.SendCompleted += client_SendCompleted;
            client.ReceiveCompleted += client_ReceiveCompleted;
            client.DisconnectCompleted += client_DisconnectCompleted;

            socket.BeginAccept(AcceptCallback, null);
            clients.Add(client);
            if(AcceptCompleted!=null)
            {
                AcceptCompleted(this, new SocketEventArgs(client, SocketAsyncOperation.Accept));
            }
        }

        private void client_SendCompleted(object sender, SocketEventArgs e)
        {
            if(SendCompleted!=null)
            {
                SendCompleted(this, e);
            }
        }

        private void client_ReceiveCompleted(object sender,SocketEventArgs e)
        {
            if(ReceiveCompleted!=null)
            {
                ReceiveCompleted(this, e);
            }
        }

        private void client_DisconnectCompleted(object sender,SocketEventArgs e)
        {
            clients.Remove((TCPListenerClient)e.Socket);

            e.Socket.DisconnectCompleted -= client_DisconnectCompleted;
            e.Socket.ReceiveCompleted -= client_ReceiveCompleted;
            e.Socket.SendCompleted -= client_SendCompleted;
            if(DisconnectCompleted!=null)
            {
                DisconnectCompleted(this, e);
            }
        }

        public void Stop()
        {
            if(!IsStarted)
            {
                throw new InvalidOperationException("没有开始服务。");
            }

            foreach(TCPListenerClient client in clients)
            {
                client.Disconnect();
                client.DisconnectCompleted -= client_DisconnectCompleted;
                client.SendCompleted -= client_SendCompleted;
                client.ReceiveCompleted -= client_ReceiveCompleted;
            }
            socket.Close();
            socket = null;
            IsStarted = false;
        }

        public void Dispose()
        {
            if(socket ==null)
            {
                return;
            }
            Stop();
        }
    }

   
}
