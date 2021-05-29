﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffie_Hellnah
{
    class Prime_Number
    {
        public long g, p;
        
        public long generator(long p)
        {
            List<long> fact = new List<long>();
            long phi = p - 1;
            long n = phi;
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

            for (long res = 2; res <= p; res++)
            {
                bool ok = true;
                for (int i = 0; i < fact.Count && ok; i++)
                {
                    ok &= power(res, phi / fact[i], p) != 1;
                }
                if (ok)
                    return res;
            }
            return -1;
        }

        public static long power(long x, long y, long p)
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
        public static bool MillerRabin(long q, long n)
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

        public static bool isPrime(long n)
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

        public static long RandomPrime(int size)
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

        public void generate_pair(int size)
        {
            p = RandomPrime(size);
            g = generator(p);
        }

    }
}