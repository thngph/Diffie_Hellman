﻿using System;
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
        public static string Caesar(long key, string msg)
        {
            string cipher = "";
            foreach (char ch in msg)
            {
                if (!char.IsLetter(ch))
                    cipher = cipher + " ";
                if (char.IsUpper(ch))
                {
                    int i;
                    for (i = 0; i < upp.Length; i++)
                    {
                        if (ch.ToString().Equals(upp[i]))
                            break;
                    }
                    cipher = cipher + upp[(i - key) % 26];
                }
                if (char.IsLower(ch))
                {
                    int i;
                    for (i = 0; i < low.Length; i++)
                    {
                        if (ch.ToString().Equals(low[i]))
                            break;
                    }
                    cipher = cipher + low[(i - key) % 26];
                }
            }
            return cipher;
        }

        public static string Viginnere(long k, string msg)
        {
            string key = k.ToString();
            string cipher = "";
            while (key.Length < msg.Length)
                key = key + key;
            int i = 0;

            foreach (char ch in msg)
            {
                if (!char.IsLetter(ch))
                {
                    key = key.Insert(i, " ");
                }
                i++;
            }
            Console.WriteLine(key);
            int i_key = 0;
            foreach (char ch in msg)
            {
                if (!char.IsLetter(ch))
                    cipher = cipher + " ";

                if (char.IsUpper(ch))
                {
                    char a = key[i_key];
                    int subkey = Int32.Parse(a.ToString());
                    for (i = 0; i < upp.Length; i++)
                    {
                        if (ch.ToString().Equals(upp[i]))
                            break;
                    }
                    cipher = cipher + upp[(i - subkey) % 26];
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
                    cipher = cipher + low[(i - subkey) % 26];
                }
                i_key++;
            }
            return cipher;
        }
        public static string AES_ECB(string text,long k)
        {
            byte[] src = Convert.FromBase64String(text);
            RijndaelManaged aes = new RijndaelManaged();
            byte[] key = Encoding.ASCII.GetBytes(k.ToString());
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