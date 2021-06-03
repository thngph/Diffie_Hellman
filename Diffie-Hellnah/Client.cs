using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Diffie_Hellnah
{
    public partial class Client : Form
    {
        public long p = 0, g = 0, A = 0, B = 0;
        private long a = 0, Ka = 0;
        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            start();
        }

        private Socket _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public object _currentData;
        public const int _buffer = 1024;
        int port;
        IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint iPEndPoint;

        private void start()
        {
            port = 8080;
            iPEndPoint = new IPEndPoint(iPAddress, port);
            while (true)
            {
                try
                {
                    _client.Connect(iPEndPoint);
                    Thread listen = new Thread(Receive);
                    listen.IsBackground = true;
                    listen.Start();
                    break;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    port++;
                }
            }

        }

        private void Send(string data_need_to_send)
        {
            try
            {
                _client.Send(Serialize(data_need_to_send));

            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
        }
        private void Receive()
        {
            _currentData = new object();
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[_buffer * 5];
                    _client.Receive(buffer);
                    _currentData = Deserialize(buffer);
                    string data = _currentData as string;
                    msg_checker(data);
                    listView1.Items.Add(data);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            
            if(textBox1.Text.ToLower().Contains("public key a"))
            {
                Send("Alice [the client]: " + textBox1.Text);
                string text = "";
                a = Key_Exc.NextLong(new Random(), 1, g);
                Thread.Sleep(200);
                listView1.Items.Add(">> a = " + a.ToString());
                A = Prime_Number.power(g, a, p);
                listView1.Items.Add(">> generated public key A");
                text = String.Format("Alice [the client]: I have sent you my public key! A = {0}", A);
                Send(text);
            }
            else
            {
                string text_sended = "Alice [the client]: " + textBox1.Text;
                Send(text_sended);
            }
        }

        byte[] Serialize(object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, o);
            return ms.ToArray();
        }
        object Deserialize(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(ms);
        }


        int msg_checker(string msg)
        {
            if(msg.Contains("p ="))
            {
                p = Lnumber_extracter(msg);
                Send(String.Format("Alice [the client]: I have received the random Prime number p!"));
                return 1;
            }
            if (msg.Contains("g ="))
            {
                g = Lnumber_extracter(msg);
                Send(String.Format("Alice [the client]: I have received g!"));
                return 2;
            }
            if (msg.Contains("B ="))
            {
                B = Lnumber_extracter(msg);
                Send("Alice [the client]: I have received your public key too!");
                
                Ka = Prime_Number.power(B, a, p);
                Thread.Sleep(200);
                listView1.Items.Add(">> key exchanged successfully!");
            }
            return -1;
        }

        int number_extracter(string msg)
        {
            string b = "";

            for (int i = 0; i < msg.Length; i++)
            {
                if (Char.IsDigit(msg[i]))
                    b += msg[i];
            }

            if (b.Length > 0)
                return int.Parse(b);
            return -1;
        }

        long Lnumber_extracter(string msg)
        {
            string b = "";

            for (int i = 0; i < msg.Length; i++)
            {
                if (Char.IsDigit(msg[i]))
                    b += msg[i];
            }

            if (b.Length > 0)
                return long.Parse(b);
            return -1;
        }
        private void Client_Load(object sender, EventArgs e)
        {

        }

        
    }
}
