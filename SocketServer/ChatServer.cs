using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.IO;

namespace SocketServer
{
    class ChatServer
    {
        public TcpListener listenObj;//监听对象
        public Dictionary<string, TcpClient> clientMem = new Dictionary<string, TcpClient>(); //客户端列表一定要初始化,new

        private Thread listenThread; //监听线程
        public delegate void ConnectDelegate(); //连接成功后处理事件的方法类型
        public event ConnectDelegate ConnectEvent;//连接事件
        public delegate void ReceiveDelegate(string message); //接收数据后处理事件方法类型
        public event ReceiveDelegate ReceiveEvent; //接收数据事件

        public void Listen(int port) //监听方法，启动监听线程
        {
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName()); //通过主机名得到本地IP
            this.listenObj = new TcpListener(localIP[1], port); //监听对象
            this.listenThread = new Thread(ListenClient); //这个线程仅仅用来监听客户        
            listenThread.Start();//启动这个线程方法          
        }

        public void ListenClient()  //监听线程对应的方法,监听到信息后向所有的客户端发送数据
        {
            while (true) //一直监听，可以有多个客户端请求连接
            {
                listenObj.Start();//开始侦听请求 ;注意在线程start之后才可以。
                TcpClient acceptClientObj = listenObj.AcceptTcpClient();//接收挂起的连接请求，这是一个阻塞方法                  
                this.ConnectEvent();//触发连接事件
                Thread receiveThread = new Thread(Receiver); //这个线程处理接收的数据
                string connectTime = DateTime.Now.ToString();
                receiveThread.Name = connectTime;//设置线程的名字
                this.clientMem.Add(connectTime, acceptClientObj); //将 客户 添加到列表     
                receiveThread.Start();//接收到的连接包含数据               
            }
        }


        public void Send(string message) //发送信息
        {

            foreach (KeyValuePair<string, TcpClient> var in clientMem) //向所有客户发送数据
            {

                if (var.Value == null || var.Value.Connected == false)
                {
                    clientMem.Remove(var.Key);  //删除断开的连接？？？这个地方有待改进
                    continue;
                }

                NetworkStream ns = var.Value.GetStream();//得到网络流
                StreamWriter sw = new StreamWriter(ns);
                sw.WriteLine(message);
                sw.Flush();//刷新数据流  
                ns.Flush();
            }

        }
        public void Receiver()  //接收 数据 对应的方法
        {   //所有的TcpClient都对应一个线程，用来接收客户端发来的数据，通过线程名，找到对应的TcpClient

            while (true)
            {
                //收到一个TcpClient时，都有一个命名的Thread对应，
                NetworkStream ns = clientMem[Thread.CurrentThread.Name].GetStream();
                StreamReader sr = new StreamReader(ns);
                string message = sr.ReadLine();//读取消息    
                this.ReceiveEvent(message);//接收过数据 就触发接收消息的事件            
            }
        }
    }
}
