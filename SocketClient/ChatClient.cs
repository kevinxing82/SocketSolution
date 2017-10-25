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
    class ChatClient
    {
        public TcpClient tcpClientObj;//收发数据
        private Thread receiveThread; //接收数据的线程
        public delegate void receiveDelegate(string receiveData);//处理接收数据事件的方法类型
        public event receiveDelegate receiveEvent; //接收数据的事件

        public void SendConnection(string ip, int port)  //通过IP地址和端口号发送连接
        {
            IPAddress ipaddr = IPAddress.Parse(ip);//转为IP地址后在连接会加快速度
            tcpClientObj = new TcpClient(); //连接客户端 
            tcpClientObj.Connect(ipaddr, port);//连接    
            receiveThread = new Thread(Receiver); //启动接收数据线程
            receiveThread.Start();
        }

        public void Send(string message) //发送信息
        {
            if (tcpClientObj == null)
            {
                return;
            }
            NetworkStream ns = this.tcpClientObj.GetStream();//得到网络流
            StreamWriter sw = new StreamWriter(ns);
            sw.WriteLine(message);//发送信息
            sw.Flush();//使所有的缓冲数据写入基础数据流  
            ns.Flush();
        }

        private void Receiver()  //接收数据对应的线程(接收到数据以后触发事件)
        {
            while (true) //一直接受
            {
                NetworkStream ns = this.tcpClientObj.GetStream();
                StreamReader sr = new StreamReader(ns);
                string receivedata = sr.ReadLine();
                receiveEvent(receivedata);//触发事件
            }
        }
    }
}
