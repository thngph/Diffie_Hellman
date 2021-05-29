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
        public long g, p, B;
        private long b, Kb;
        public Server()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string text = "Bob [the server]" + ": " + textBox1.Text;

            string text_sended =  text;

            Send(text_sended);
            listView1.Items.Add(text);
        }

        string _currentData = "";
        public const int _buffer = 1024;
        private Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private List<Socket> _clients = new List<Socket>();

        private void button1_Click(object sender, EventArgs e)
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
            listView1.Items.Add("Server is running...");
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
                    char[] b = { ';' };
                    int count = 3;
                    String[] strList = data.Split(b, count, StringSplitOptions.RemoveEmptyEntries);
                    if (strList.Length >= 2)
                    {
                        Send(strList[0] + "; " + strList[1]);
                        listView1.Items.Add(strList[0] + strList[1]);
                    }
                    else
                    {
                        Send(data);
                        listView1.Items.Add(data);
                    }
                    if (data.Contains(";"))
                    {

                        


                        if (strList[0].Contains("p ="))
                        {
                            string bb = "";

                            for (int i = 0; i < strList[0].Length; i++)
                            {
                                if (Char.IsDigit(strList[0][i]))
                                    bb += strList[0][i];
                            }

                            if (bb.Length > 0)
                            {
                                p = int.Parse(bb);
                                this.b = Key_Exc.LongRandom(0, p, new Random());
                                listView1.Items.Add(">> b: " + this.b.ToString());
                                B = Prime_Number.power(g, this.b, p);

                                
                                Kb = Prime_Number.power(g, long.Parse(bb), p);
                            }

                        }

                        if (strList[1].Contains("g ="))
                        {
                            string bb = "";

                            for (int i = 0; i < strList[1].Length; i++)
                            {
                                if (Char.IsDigit(strList[1][i]))
                                    bb += strList[1][i];
                            }

                            if (bb.Length > 0)
                            {
                                g = int.Parse(bb);
                            }
                        }

                        if (strList[2].Length != 0)
                        {
                            this.B = Prime_Number.power(g,this.b, p);
                            Send("sharekey " + B.ToString());
                            listView1.Items.Add(">> shared key B sent.");
                        }
                    }

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

        public int powmod(int a, int b, int p)
        {
            int res = 1;
            while (b != 0)
            {
                if ((b & 1) != 0)
                {
                    res = (int)(res * 1L * a % p);
                    --b;
                }
                else
                {
                    a = (int)(a * 1L * a % p);
                    b >>= 1;
                }

            }
            return res;
        }


        public int generator(int p)
        {
            List<int> fact = new List<int>();
            int phi = p - 1;
            int n = phi;
            for (int i = 2; i * i <= n; i++)
            {
                if (n % i == 0)
                {
                    fact.Add(i);
                    while (n % i == 0)
                        n /= i;
                }
            }
            if (n > 1)
            {
                fact.Add(n);
            }

            for (int res  = p -1; res >=2; res--)
            {
                bool ok = true;
                for (int i = 0; i < fact.Count && ok; i++)
                {
                    ok &= powmod(res, phi / fact[i], p) != 1;
                }
                if (ok)
                    return res;
            }
            return -1;
        }
    }



}
