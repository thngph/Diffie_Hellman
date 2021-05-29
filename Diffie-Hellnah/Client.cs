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
        public long p, g, A;
        private long a, Ka;
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
                    if (data.Contains("sharekey"))
                    {
                        string B = "";

                        for (int i = 0; i < data.Length; i++)
                        {
                            if (Char.IsDigit(data[i]))
                                B += data[i];
                        }
                        Ka = Prime_Number.power(long.Parse(B),a,p);
                        listView1.Items.Add(">>public key B received.");
                    }
                    else
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
            string text = "";
            if(textBox1.Text =="generate pair")
            {
                listView1.Items.Add(">>" + textBox1.Text);
                Prime_Number pn = new Prime_Number();
                pn.generate_pair(4*8);
                p = pn.p;
                g = pn.g;
                this.a = Key_Exc.NextLong(new Random(), 0, p - 1);
                A = Prime_Number.power(g, this.a, p);

                listView1.Items.Add(">> a: " + a);
                listView1.Items.Add("shared key a sent.");
                //p = small_primes[rnd.Next(small_primes.Length)].ToString();
                //int size_bits = 10;
                //p = RandomPrime(10).ToString();


                text = textBox2.Text + "[the client]: p = " + pn.p.ToString() + "; g = " + pn.g.ToString() +";" + A.ToString() ;


            }
            else
            {
                text = textBox2.Text + "[the client]: " + textBox1.Text;
            }
            string text_sended =  text;

            Send(text_sended);
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

        public int powmod (int a, int b, int p)
        {
            int res = 1;
            while ( b != 0)
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


        
        private void Client_Load(object sender, EventArgs e)
        {

        }

        
    }
}
