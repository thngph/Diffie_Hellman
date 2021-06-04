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
        public int type = 0;
        string str_tmp;
        public Server()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            StartListen();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            text = "Bob [the server]" + ": " + text;
            if (type == 0)
            {
                listView1.Items.Add(text);
                Send(text);
            }
            else
            {
                string msg = textBox1.Text;
                string cipher = encrypt_msg(type, msg);
                //listView1.Items.Add("Bob [the server]:" +  cipher);
                listView1.Items.Add("Bob [the server][secured message]: " + decrypt_msg(type,cipher));
                Send(String.Format("Bob [the server][secured message]:" + cipher));
            }

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
                    if (type == 0)
                    {
                        Send(data);
                        listView1.Items.Add(data);
                        msg_checker(data);                       
                    }
                    else
                    {
                        Send(data);
                        msg_checker(data);                                              
                    }
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
            if (type == 0 )
            {
                if (msg.Contains("bit"))//nếu lời gửi qua có chứa bits =>tạo p
                {
                    int sz = number_extracter(msg);
                    p = Prime_Number.RandomPrime(sz);
                    listView1.Items.Add(">> p = " + p.ToString());
                    Send(String.Format("Bob [the server]: I have sent you a random Primary number p = {1}", sz, p));
                    b = Kb = B = 0;
                    return 1;
                }
          

                if (msg.ToLower().Contains("send me g") | (msg.Contains("G")))//nếu lời gửi qua có chứa send me g=> tạo g
                {
                    g = Prime_Number.generator(p);
                    listView1.Items.Add(">> g = " + g.ToString());
                    Send(String.Format("Bob [the server]: I have sent you a primitive root of p, g = {0}", g));
                    return 2;
                }

                else if (msg.ToLower().Contains("a =")) //nếu lời gửi qua có chứa a 
                {
                    A = Lnumber_extracter(msg); // Tách lấy phần số kiểu long trong message được gửi từ Alice
                    Send(String.Format("Bob [the server]: I have received your public key!"));

                    b = Key_Exc.NextLong(new Random(), 1, p);//Tạo khóa private của Bob 
                    listView1.Items.Add(">> b = " + b.ToString());

                    B = Prime_Number.power(g, b, p); // Tính khóa public của Bob
                    listView1.Items.Add(">> generated public key B");
                    listView1.Items.Add(">> B = " + B.ToString());
                    string text = String.Format("Bob [the server]: I have sent you my public key! B = {0}", B);
                    Send(text);//gửi

                    Kb = Prime_Number.power(A, b, p);//Tính khóa chung bằng công thưc Kb=A^b mod p
                    listView1.Items.Add(">> key exchanged successfully!");
                    listView1.Items.Add(">> K = " + Kb.ToString());

                    return 3;
                }

                else if (msg.ToLower().Contains("encrypt")  | msg.ToLower().Contains("start secured message") | msg.ToLower().Contains("enc"))//Chọn loại mã hóa
                {
                    Send(String.Format("Bob [the server]: Select 1 out of the following encrypting options: 1. Caesar, 2. Viginnere, 3. AES-ECB"));
                    listView1.Items.Add("Bob [the server]: Select 1 out of the following encrypting options: 1. Caesar, 2. Viginnere, 3. AES-ECB");
                    return 4;
                }

                else if (msg.ToLower().Contains("choose") | msg.ToLower().Contains("opt"))
                {
                    if (msg.ToLower().Contains("1"))
                    {
                        type = 1;
                        listView1.Items.Add(">> Message will be encrypted under Caesar");
                    }
                    if (msg.ToLower().Contains("2"))
                    {
                        type = 2;
                        listView1.Items.Add(">> Message will be encrypted under Viginnere");
                    }
                    if (msg.ToLower().Contains("3"))
                    {
                        type = 3;
                        listView1.Items.Add(">> Message will be encrypted under AES_ECB");
                    }
                    return 5;
                }
            }
            else
            {
                char[] b = { ':' };
                int count = 2;
                String[] strList = msg.Split(b, count, StringSplitOptions.RemoveEmptyEntries);
                //listView1.Items.Add(msg);
                str_tmp = strList[0] + "[secured message]: " + decrypt_msg(type, strList[1]);
                listView1.Items.Add(str_tmp);
                return 6;
            }
            return -1;
        }

        string encrypt_msg(int type,string msg)
        {
            string cipher = "";
            if(type==1)
            {
                cipher = Encrypt.Caesar(Kb, msg);
            }
            else if(type==2)
            {
                cipher = Encrypt.Viginnere(Kb, msg);
            }
            else if(type==3)
            {
                cipher = Encrypt.AES_ECB(msg, Kb);
            }
            return cipher;
        }

        string decrypt_msg(int type, string msg)
        {

            string plaintext = "";
            if (type == 1)
            {
                plaintext = Decrypt.Caesar(Kb, msg);
            }
            else if (type == 2)
            {
                plaintext = Decrypt.Viginnere(Kb, msg);
            }
            else if (type == 3)
            {
                plaintext = Decrypt.AES_ECB(msg, Kb);
            }
            return plaintext;
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
