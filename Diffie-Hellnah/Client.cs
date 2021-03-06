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
        public int type = 0;
        public string str_tmp;
        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            start();
            listView1.Items.Add(">> send 'help' to get secured chat's instructions");
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
                    if (type == 0)
                    {

                        listView1.Items.Add(data);
                        msg_checker(data);
                                            
                    }
                    else
                    {
                        char[] b = { ':' };
                        int count = 2;
                        String[] strList = data.Split(b, count, StringSplitOptions.RemoveEmptyEntries);
                        string _data = strList[1];
                        if (_data.Contains("choose 1") | _data.Contains("choose 2") | _data.Contains("choose 3") | _data.Contains("opt 1") | _data.Contains("opt 2") | _data.Contains("opt 3"))
                        {
                            string special = "[secured message]";
                            if (strList[0].ToLowerInvariant().Contains(special))
                            {
                                strList[0] = strList[0].Replace(special, "");
                            }
                            listView1.Items.Add(strList[0] + ": " + _data);
                        }
                           
                        else
                        {
                            msg_checker(data);

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string txt1 = textBox1.Text;
            textBox1.Text = "";
            if(txt1.ToLower() == "help")
            {
                MessageBox.Show("TYPE IN STRINGS CONTAIN PHRASES BELOW:\n\n*size + bits: generate random *size bits prime number\nG: get the largest primitive root g of p\nexc: exchange key(s)\nenc: start encrypted chat with exchanged shared key\n\notherwise. the chat remains PUBLIC");
                txt1 = "";
                return;
            }
            if (type == 0)
            {

                if (txt1.ToLower().Contains("exchange key") | (txt1.ToLower().Contains("exc")))
                {
                    Send("Alice [the client]: " + txt1);
                    string text = "";

                    a = Key_Exc.NextLong(new Random(), 1, g);
                    A = Prime_Number.power(g, a, p);

                    text = String.Format("Alice [the client]: I have sent you my public key! A = {0}", A);
                    Send(text);
                    listView1.Items.Add(">> a = " + a.ToString());
                    listView1.Items.Add(">> generated public key A");
                    return;
                }

                else if (txt1.ToLower().Contains("choose") | txt1.ToLower().Contains("opt"))
                {
                    if (txt1.ToLower().Contains("1"))
                    {
                        type = 1;
                        Send(String.Format("Alice [the client]: " + txt1));
                        listView1.Items.Add(">> Message will be encrypted under Caesar cipher");
                        return;
                    }
                    if (txt1.ToLower().Contains("2"))
                    {
                        type = 2;
                        Send(String.Format("Alice [the client]: " + txt1));
                        listView1.Items.Add(">> Message will be encrypted under Viginnere cipher");
                        return;
                    }
                    if (txt1.ToLower().Contains("3"))
                    {
                        type = 3;
                        Send(String.Format("Alice [the client]: " + txt1));
                        listView1.Items.Add(">> Message will be encrypted under AES-ECB cipher");
                        return;
                    }
                    else {
                        listView1.Items.Add(">> please use a valid value (such as 'choose 1'/ 'opt 1')");
                    }
                }
                else
                {
                    string text_sended = "Alice [the client]: " + txt1;
                    Send(text_sended);
                }
            }
            else if (type != 0)
            {
                string msg = txt1;
                string cipher = encrypt_msg(type, msg);
                Send(String.Format("Alice [the client][secured messaged]:"+ cipher));
                
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
            if (type == 0)
            {
                if (msg.Contains("p ="))
                {
                    p = Lnumber_extracter(msg);//Trích p từ tin nhắn của p
                    Send(String.Format("Alice [the client]: I have received the random Prime number p!"));
                    a = A = Ka = 0;
                    return 1;
                }

                if (msg.Contains("g ="))
                {
                    g = Lnumber_extracter(msg);//Trích g từ tin nhắn của Bob
                    Send(String.Format("Alice [the client]: I have received g!"));
                    return 2;
                }

                if (msg.Contains("B ="))
                {
                    B = Lnumber_extracter(msg);//Trích B từ tin nhắn của Bob
                    Send("Alice [the client]: I have received your public key too!");

                    Ka = Prime_Number.power(B, a, p);
                    listView1.Items.Add(">> key exchanged successfully!");
                    listView1.Items.Add(">> K = " + Ka.ToString());
                    return 3;
                }
            }

            else
            {
                char[] b = { ':' };
                int count = 2;
                String[] strList = msg.Split(b, count, StringSplitOptions.RemoveEmptyEntries);
                //listView1.Items.Add(msg);
                str_tmp = strList[0] + ": " + decrypt_msg(type, strList[1]);
                listView1.Items.Add(str_tmp);
            }
            return -1;
        }
        string encrypt_msg(int type, string msg)
        {
            string cipher = "";
            if (type == 1)
            {
                cipher = Encrypt.Caesar(Ka, msg);
            }
            else if (type == 2)
            {
                cipher = Encrypt.Vigenere(Ka, msg);
            }
            else if (type == 3)
            {
                cipher = Encrypt.AES_ECB(msg, Ka);
            }

            return cipher;

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        string decrypt_msg(int type, string msg)
        {

            string plaintext = "";
            if (type == 1)
            {
                plaintext = Decrypt.Caesar(Ka, msg);
            }
            else if (type == 2)
            {
                plaintext = Decrypt.Vigenere(Ka, msg);
            }
            else if (type == 3)
            {
                plaintext = Decrypt.AES_ECB(msg, Ka);
            }
            return plaintext;
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
        long power(long x, long y, long p)
        {
            long res = 1;
            while (y != 0)
            {
                if (y % 2 == 1)
                {
                    res = (res * 1L * x) % p; --y;
                }
                else
                {
                    x = (x * 1L * x % p); y >>= 1;
                }
            }
            return res;
        }
            private void Client_Load(object sender, EventArgs e)
        {

        }

        
    }
}
