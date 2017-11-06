using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace SocketServer
{
    public partial class ServerForm : Form
    {
        ChatServer serverObj = new ChatServer();
        public ServerForm()
        {
            InitializeComponent();
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
            try
            {
                serverObj.ConnectEvent += new ChatServer.ConnectDelegate(serverObj_ConnectEvent);  //订阅连接事件

                serverObj.ReceiveEvent += new ChatServer.ReceiveDelegate(serverObj_ReceiveEvent);  //订阅接收数据事件

                serverObj.Listen(Convert.ToInt32(tbPort.Text)); //启动监听
                this.lbMessage.Items.Add("启动监听");
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载失败：" + ex.Message);
                return;
            }

        }

        void serverObj_ReceiveEvent(string message)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    ChatServer.ReceiveDelegate update = new ChatServer.ReceiveDelegate(serverObj_ReceiveEvent);

                    this.Invoke(update, new object[] { message });
                }
                else
                {
                    this.lbMessage.Items.Add(message);//添加到显示栏
                    serverObj.Send(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理事件方法错误：" + ex.Message);
                return;
            }

        }

        void serverObj_ConnectEvent()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    ChatServer.ConnectDelegate update = new ChatServer.ConnectDelegate(serverObj_ConnectEvent);

                    this.Invoke(update);
                }

                else
                {
                    this.lbMessage.Items.Add("连接成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理连接事件方法错误:" + ex.Message);
                return;
            }
        }

        private void ServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.lbMessage.Items.Add("关闭服务器。。。");
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
