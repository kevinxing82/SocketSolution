using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketClient
{
    public partial class ClientForm : Form
    {
        private ChatClient clientObj = new ChatClient(); //客户对象,对于一个对象一定要new啊
        public ClientForm()
        {
            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            btnSend.Enabled = false; //没有连接不允许发送数据
            this.AcceptButton = btnSend;

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string nickName = tbName.Text;
            string ip = tbIP.Text;
            string port = tbPort.Text;

            if (string.IsNullOrEmpty(nickName) || string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port))
            {
                MessageBox.Show("请将昵称、IP填写完整");
                return;
            }

            try
            {
                clientObj.SendConnection(ip, Convert.ToInt32(port)); //连接  
                clientObj.receiveEvent += new ChatClient.receiveDelegate(ClientObj_receiveEvent); //订阅事件的处理方法
                clientObj.Send(tbName.Text + "登陆成功!");
                btnSend.Enabled = true;
                btnConnect.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接时出错:" + ex.Message);
                return;
            }
        }

        void ClientObj_receiveEvent(string receiveData)
        {
            try
            {
                if (this.InvokeRequired) //指示是否需要在这个线程上调用方法
                {
                    ChatClient.receiveDelegate update = new ChatClient.receiveDelegate(ClientObj_receiveEvent);//当把消息传递给控件线程时重复调用该方法就会调用else
                    this.Invoke(update, new object[] { receiveData });//将消息发送给控件线程处理
                }
                else
                {
                    lbMessage.Items.Add(receiveData);//添加数据
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("接收数据错误：" + ex.Message);
                return;
            }

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbMessage.Text))
                {
                    return;
                }
                clientObj.Send(tbName.Text + "说:" + tbMessage.Text);//发送信息
                tbMessage.Clear();//清除原来的文本
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送数据出错:" + ex.Message);
                return;
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ClientObj.Send(this.tbName.Text + "下线了");
            //ClientObj.tcpClientObj.Close();//关闭连接
            this.Close();//关闭窗体，让程序自动释放资源
        }
    }
}
