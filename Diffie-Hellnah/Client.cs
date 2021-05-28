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
        public string p;
        public string g;

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
            if(textBox1.Text =="get p")
            {
                //p = small_primes[rnd.Next(small_primes.Length)].ToString();
                //int size_bits = 10;
                p = RandomPrime(10).ToString();
                text = textBox2.Text + "[the client]: p =" + p;
                
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


        public int generator(int p)
        {
            List<int> fact = new List<int>();
            int phi = p - 1;
            int n = phi;
            for(int i = 2; i*i <= n; i++)
            {
                if (n%i == 0)
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
            
            for (int res = 2; res <= p; res++)
            {
                bool ok = true;
                for(int i = 0; i < fact.Count && ok; i++)
                {
                    ok &= powmod(res, phi / fact[i], p)!= 1;
                }
                if (ok)
                    return res;
            }
            return -1;
        }

        
        private void Client_Load(object sender, EventArgs e)
        {

        }

        public static Random rnd = new Random();
        public static int[] small_primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };

        static long power(long x, long y, long p)
        {
            long res = 1;
            x = x % p;
            while (y > 0)
            {
                if (y % 2 == 1)
                {
                    res = (res * x) % p;
                }
                y = y >> 1;
                x = (x * x) % p;
            }
            return res;
        }
        static bool MillerRabin(long q, long n)
        {
            Random r = new Random();
            int k = (int)n - 2;
            long a = r.Next(2, k);
            long x = power(a, q, n);
            if (x == 1 || x == n - 1)
                return true;
            while (q != n - 1)
            {
                x = (x * x) % n;
                q *= 2;

                if (x == 1)
                    return false;
                if (x == n - 1)
                    return true;
            }
            return false;
        }

        static bool isPrime(long n)
        {
            if (n <= 1 || n == 4)
                return false;
            if (n <= 3)
                return true;

            long q = n - 1;
            while (q % 2 == 0)
                q /= 2;

            for (int i = 1; i <= 50; i++)
            {
                if (MillerRabin(q, n) == false)
                    return false;
            }

            return true;
        }

        static long RandomPrime(int size)
        {
            Random r = new Random();
            int a = (int)(Math.Pow(2, size - 1));
            int b = (int)(Math.Pow(2, size));
            long beg_rand = r.Next(a, b);
            if (beg_rand % 2 == 0)
                beg_rand += 1;

            for (long possiblePrime = beg_rand; possiblePrime <= b; possiblePrime++)
            {
                if (isPrime(possiblePrime))
                {
                    return possiblePrime;
                }
            }
            return 0;
        }


    }
}
