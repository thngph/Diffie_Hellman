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
using System.Windows.Forms;

namespace Diffie_Hellnah
{
    
    public partial class Server : Form
    {
        public long g = 0, p = 0, B = 0, A = 0;
        private long b = 0, Kb = 0;
        public Server()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            StartListen();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            listView1.Items.Add(text);
            text = "Bob [the server]" + ": " + text;
            Send(text);
            
        }

        string _currentData = "";
        public const int _buffer = 1024;
        private Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private List<Socket> _clients = new List<Socket>();

        private void button1_Click(object sender, EventArgs e)
        {

        }

        void StartListen()
        {

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 8080);
            server.Bind(iPEndPoint);

            Thread listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        _clients.Add(client);
                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            listen.IsBackground = true;
            listen.Start();
            button1.Enabled = false;
            listView1.Items.Add("Server is ready...");
        }

        public void Receive(object obj)
        {
            try
            {
                Socket client = obj as Socket;
                _currentData = "";

                while (true)
                {
                    byte[] buffer = new byte[_buffer * 5];
                    client.Receive(buffer);
                    _currentData = (string)Deserialize(buffer);
                    string data = _currentData as string;
                    Send(data);
                    listView1.Items.Add(data);

                    msg_checker(data);

                    //char[] b = { ';' };
                    //int count = 3;
                    //String[] strList = data.Split(b, count, StringSplitOptions.RemoveEmptyEntries);
                  

                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }
        }


        private void Send(string data_need_to_send)
        {
            try
            {
                foreach (Socket client in _clients)
                {
                    client.Send(Serialize(data_need_to_send));
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
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
            if (msg.Contains("bit"))
            {
                int sz = number_extracter(msg);
                p = Prime_Number.RandomPrime(sz);
                listView1.Items.Add(">> p = " + p.ToString());
                Send(String.Format("Bob [the server]: I have sent you a random {0} bits Primary number p = {1}", sz, p ));
                return 1;
            }
            if(msg.ToLower().Contains("send me g"))
            {
                g = Prime_Number.generator(p);
                listView1.Items.Add(">> g = " + g.ToString());
                Send(String.Format("Bob [the server]: I have sent you a primitive root of p, g = {0}", g));
                return 2;
            }
            if (msg.ToLower().Contains("a ="))
            {
                A = Lnumber_extracter(msg);
                Send(String.Format("Bob [the server]: I have received your public key!"));
                b = Key_Exc.NextLong(new Random(), 1, p);
                listView1.Items.Add(">> b = " + b.ToString());
                B = Prime_Number.power(g, b, p);
                listView1.Items.Add(">> generated public key B");
                string text = String.Format("Bob [the server]: I have sent you my public key! B = {0}", B);
                Send(text);
                Kb = Prime_Number.power(A,b,p);
                listView1.Items.Add(">> key exchanged successfully!");
                return 3;
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
    }



}
