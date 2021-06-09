using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace Diffie_Hellnah
{
    class Decrypt
    {
        public static string[] low = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        public static string[] upp = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public static string[] number = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
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
                            long tmp = i - key % 10;
                            if (tmp < 0) tmp = tmp + 10;
                            cipher = cipher + number[tmp];
                            state = true;
                            break;
                        }
                    }
                    if (!state)
                        cipher = cipher + ch;
                }
                

                if (char.IsUpper(ch))
                {
                    int i;
                    for (i = 0; i < upp.Length; i++)
                    {
                        if (ch.ToString().Equals(upp[i]))
                            break;
                    }
                    long tmp = i - key % 26;
                    if (tmp < 0) tmp = tmp + 26;
                    cipher = cipher + upp[tmp];
                }
                if (char.IsLower(ch))
                {
                    int i;
                    for (i = 0; i < low.Length; i++)
                    {
                        if (ch.ToString().Equals(low[i]))
                            break;
                    }
                    long tmp = i - key%26;
                    if (tmp < 0) tmp = tmp + 26;
                    cipher = cipher + low[tmp];
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
                            //cipher = cipher + number[(subkey + j) % 10];
                            long tmp = j - subkey % 10;
                            if (tmp < 0) tmp = tmp + 10;
                            cipher = cipher + number[tmp];
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
                    long tmp = i - subkey % 26;
                    if (tmp < 0) tmp = tmp + 26;
                    cipher = cipher + upp[tmp];
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
                    long tmp = i - subkey % 26;
                    if (tmp < 0) tmp = tmp + 26;
                    cipher = cipher + low[tmp];
                }
                i_key++;
            }
            return cipher;
        }
        public static string AES_ECB(string text,long k)
        {
            string k_string = k.ToString();
            int padding = 16 - k_string.Length;
            if (k_string.Length != 16)
            {
                for (int i = 0; i < padding; i++)
                    k_string = k_string + "x";
            }
            byte[] src = Convert.FromBase64String(text);
            RijndaelManaged aes = new RijndaelManaged();
            byte[] key = Encoding.ASCII.GetBytes(k_string);
            aes.KeySize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.ECB;
            using (ICryptoTransform decrypt = aes.CreateDecryptor(key, null))
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                decrypt.Dispose();
                return Encoding.UTF8.GetString(dest);
            }
        }
    }
}
