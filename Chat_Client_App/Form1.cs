using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
namespace Chat_Client_App
{
    public partial class Form1 : Form
    {
        Socket socket;
        EndPoint ep_local, ep_remote;
        public Form1()
        {
            InitializeComponent();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            textBox1.Text = get_local_ip();
            textBox3.Text = get_local_ip();
        }

        private String get_local_ip()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString(); 
                }
            }
            return "127.0.0.1";
        }

        private void CallBackMessage(IAsyncResult aResult)
        {
            try
            {
                int size = socket.EndReceiveFrom(aResult, ref ep_remote);
                if (size > 0)
                {
                    byte[] receivedData = new byte[1464];
                    receivedData=(byte[])aResult.AsyncState;
                    ASCIIEncoding aEncoding = new ASCIIEncoding();
                    string receivedMessage = aEncoding.GetString(receivedData);
                    listBox1.Items.Add("Waqas : " + receivedMessage);
                }
                byte[] buffer = new byte[1500];
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref ep_remote, new AsyncCallback(CallBackMessage), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = encoding.GetBytes(textBox5.Text);
                socket.Send(msg);
                listBox1.Items.Add("Umair : " + textBox5.Text);
                textBox5.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ep_local = new IPEndPoint(IPAddress.Parse(textBox1.Text), Convert.ToInt32(textBox2.Text));
                socket.Bind(ep_local);
                ep_remote = new IPEndPoint(IPAddress.Parse(textBox3.Text), Convert.ToInt32(textBox4.Text));
                socket.Connect(ep_remote);
                byte[] buffer = new byte[1500];
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref ep_remote, new AsyncCallback(CallBackMessage), buffer);
                button2.Text = "Connected...";
                button2.Enabled = false;
                textBox5.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
