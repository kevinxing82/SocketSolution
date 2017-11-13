using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;


namespace org.kevinxing.socket
{
    public class TCPListener : IDisposable
    {
        private Socket socket;
        private SocketAsyncEventArgs accetpEventArgs;
        private HashSet<TCPListenerClient> clients;
        private int port;

        public int Port
        {
            get { return port; }
            set
            {
                if(value<0||value>65535)
                {
                    throw new ArgumentOutOfRangeException(value + "不是有效端口。");
                }
                port = value;
            }
        }

        public  TCPListener()
        {
            clients = new HashSet<TCPListenerClient>();
            accetpEventArgs = new SocketAsyncEventArgs();
            accetpEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptComplete);
        }

        public void Start(string ip,int port)
        {
            if (IsStarted)
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
            startAccept();
        }

        public void Stop()
        {
            if(!IsStarted)
            {
                throw new InvalidOperationException("没有开始服务。");
            }
            foreach (TCPListenerClient client in clients)
            {
                client.Disconnect();
                client.OnDisconnectCompleted -= Client_OnDisconnectCompleted;
                client.OnSendCompleted -= Client_OnSendCompleted;
                client.OnReceiveCompleted -= Client_OnReceiveCompleted;
            }
            socket.Close();
            socket = null;
            IsStarted = false;
        }

        private void startAccept()
        {
            if(!socket.AcceptAsync(accetpEventArgs))
            {
                processAccept(accetpEventArgs);
            }
        }

        private void processAccept(SocketAsyncEventArgs e)
        {
            TCPListenerClient client = new TCPListenerClient(this,e.AcceptSocket);
            client.OnDisconnectCompleted += Client_OnDisconnectCompleted;
            client.OnReceiveCompleted += Client_OnReceiveCompleted;
            client.OnSendCompleted += Client_OnSendCompleted;
            lock(clients)
            {
                clients.Add(client);
            }
            startAccept();
        }

        private void Client_OnSendCompleted(object sender, EventArgs e)
        {
            
        }

        private void Client_OnReceiveCompleted(object sender, byte[] e)
        {
            
        }

        private void Client_OnDisconnectCompleted(object sender, EventArgs e)
        {
           
        }

        private void OnAcceptComplete(object sender,SocketAsyncEventArgs e)
        {
            processAccept(e);
        }

        public bool IsStarted { get; private set; }


        public void Dispose()
        {
            if (socket == null)
            {
                return;
            }
            Stop();
        }
    }
}
