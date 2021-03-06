using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace Diffie_Hellnah
{
    class Encrypt
    {
        public static string[] low = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        public static string[] upp = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public static string[] number = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        public static string Caesar(long key, string msg)
        {
            string cipher = "";
            foreach (char ch in msg)
            {
               
                if (!char.IsLetter(ch))
                {
                    bool state = false;
                    for (int i = 0; i < number.Length; i++)
                    {
                        if (ch.ToString().Equals(number[i]))
                        {
                            cipher = cipher + number[(i + key) % 10];
                            state = true;
                            break;
                        }   
                    }
                    if (!state)
                        cipher += ch;
                    
                }
                if (char.IsUpper(ch))
                {
                    int i;
                    for (i = 0; i < upp.Length; i++)
                    {
                        if (ch.ToString().Equals(upp[i]))
                            break;
                    }
                    cipher = cipher + upp[(i + key) % 26];
                }
                if (char.IsLower(ch))
                {
                    int i;
                    for (i = 0; i < low.Length; i++)
                    {
                        if (ch.ToString().Equals(low[i]))
                            break;
                    }
                    cipher = cipher + low[(i + key) % 26];
                }
            }
            return cipher;
        }

        public static string Vigenere(long k, string msg)
        {
            string key = k.ToString();
            string cipher = "";
            while (key.Length < msg.Length)
                key = key + key;
            int i = 0;

            foreach (char ch in msg)
            {
                int num;
                if (!char.IsLetter(ch) && (int.TryParse(ch.ToString(), out num) == false))
                {
                    key = key.Insert(i, " ");
                }
                i++;
            }

            //Console.WriteLine(key);
            int i_key = 0;
            foreach (char ch in msg)
            {
                if (!char.IsLetter(ch))
                {
                    char a = key[i_key];
                    int subkey = Int32.Parse(a.ToString());
                    bool state = false;
                    for (int j = 0; j < number.Length; j++)
                    {
                        if (ch.ToString().Equals(number[j]))
                        {
                            cipher = cipher + number[(subkey + j) % 10];
                            state = true;
                            break;
                        }
                    }
                    if (!state)
                        cipher += ch;

                }

                if (char.IsUpper(ch))
                {
                    char a = key[i_key];
                    int subkey = Int32.Parse(a.ToString());
                    for (i = 0; i < upp.Length; i++)
                    {
                        if (ch.ToString().Equals(upp[i]))
                            break;
                    }
                    cipher = cipher + upp[(i + subkey) % 26];
                }

                if (char.IsLower(ch))
                {
                    char a = key[i_key];
                    int subkey = Int32.Parse(a.ToString());
                    for (i = 0; i < low.Length; i++)
                    {
                        if (ch.ToString().Equals(low[i]))
                            break;
                    }
                    cipher = cipher + low[(i + subkey) % 26];
                }
                i_key++;
            }
            return cipher;
        }

        public static string AES_ECB(string text, long k)
        {
            string k_string = k.ToString();
            int padding = 16 - k_string.Length;
            if (k_string.Length != 16)
            {
                for (int i = 0; i < padding; i++)
                    k_string = k_string + "x";
            }
            byte[] src = Encoding.UTF8.GetBytes(text);
            byte[] key = Encoding.ASCII.GetBytes(k_string);

            RijndaelManaged aes = new RijndaelManaged();

            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;

            using (ICryptoTransform encrypt = aes.CreateEncryptor(key, null))
            {
                byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);
                encrypt.Dispose();
                return Convert.ToBase64String(dest);
            }
        }
    }
}
