using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            //Multithread Branch
            List<int> primes = new List<int> { 1 };

            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here
            

            int increment = 1;
            int primesUpTo = 10000000;
            double sum = 0;

            for (int i = 2; i <= primesUpTo; i += increment)
            {
                double limit = Math.Sqrt(i);
                int primeIndex = 1;
                bool isPrime = true;
                while (primeIndex < primes.Count() && primes[primeIndex] <= limit)
                {
                    if (i % primes[primeIndex] == 0)
                    {
                        isPrime = false;
                        break;
                    }
                    primeIndex++;
                }

                if(i > 2)
                {
                    increment = 2;
                }

                if (isPrime)
                {
                    primes.Add(i);
                    sum += i;
                    Console.WriteLine(i);
                }
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Number of primes from 1 to {primesUpTo}: {primes.Count()}");
            Console.WriteLine($"Sum of primes from 1 to {primesUpTo}: {sum}");
            Console.WriteLine($"Time to completion: {elapsedMs}ms");
        }
    }
}
