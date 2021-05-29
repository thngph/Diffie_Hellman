using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffie_Hellnah
{
    public class Encrypt
    {
        public static string[] low = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        public static string[] upp = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public static string Caesar(int key, string msg)
        {
            string cipher = "";
            foreach (char ch in msg)
            {
                if (!char.IsLetter(ch))
                    cipher = cipher + " ";
                if (char.IsUpper(ch))
                {
                    int i;
                    for(i=0;i<upp.Length;i++)
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

        public static string Viginnere(int k, string msg)
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


    }
}
