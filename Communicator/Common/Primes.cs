using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Primes
    {
        // C++ program to find primitive root of a
        // given number n

        // Returns true if n is prime
        static bool isPrime(int n)
        {
            // Corner cases
            if (n <= 1) return false;
            if (n <= 3) return true;

            // This is checked so that we can skip
            // middle five numbers in below loop
            if (n % 2 == 0 || n % 3 == 0) return false;

            for (int i = 5; i * i <= n; i = i + 6)
                if (n % i == 0 || n % (i + 2) == 0)
                    return false;

            return true;
        }

        /* Iterative Function to calculate (x^n)%p in
           O(logy) */
        static int power(int x, int y, int p)
        {
            int res = 1;     // Initialize result

            x = x % p; // Update x if it is more than or
                       // equal to p

            while (y > 0)
            {
                // If y is odd, multiply x with result
                if ((y%2) != 0)
                    res = (res * x) % p;

                // y must be even now
                y = y >> 1; // y = y/2
                x = (x * x) % p;
            }
            return res;
        }

        // Utility function to store prime factors of a number
        static void findPrimefactors(ref List<int> s, int n)
        {
            // Print the number of 2s that divide n
            while (n % 2 == 0)
            {
                s.Add(2);
                n = n / 2;
            }

            // n must be odd at this point. So we can skip
            // one element (Note i = i +2)
            for (int i = 3; i <= Math.Sqrt(n); i = i + 2)
            {
                // While i divides n, print i and divide n
                while (n % i == 0)
                {
                    s.Add(i);
                    n = n / i;
                }
            }

            // This condition is to handle the case when
            // n is a prime number greater than 2
            if (n > 2)
                s.Add(n);
        }

        // Function to find smallest primitive root of n
        public static int findPrimitive(int n)
        {
            List<int> s = new List<int>();

            // Check if n is prime or not
            if (isPrime(n) == false)
                return -1;

            // Find value of Euler Totient function of n
            // Since n is a prime number, the value of Euler
            // Totient function is n-1 as there are n-1
            // relatively prime numbers.
            int phi = n - 1;

            // Find prime factors of phi and store in a set
            findPrimefactors(ref s, phi);

            // Check for every number from 2 to phi
            for (int r = 2; r <= phi; r++)
            {
                // Iterate through all prime factors of phi.
                // and check if we found a power with value 1
                bool flag = false;
                foreach (var prime in s)
                {

                    // Check if r^((phi)/primefactors) mod n
                    // is 1 or not
                    if (power(r, phi / (prime), n) == 1)
                    {
                        flag = true;
                        break;
                    }
                }

                // If there was no power with value 1.
                if (flag == false)
                    return r;
            }

            // If no primitive root found
            return -1;
        }
    }
}
