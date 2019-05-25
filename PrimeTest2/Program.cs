using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PrimeTest2
{
    class Program
    {
        static public List<int> primes = new List<int> { 1, 2 };

        static public Dictionary<int, List<int>> newPrimes = new Dictionary<int, List<int>>();

        static public int THREAD_LIMIT = 8;

        static public void primesInRangeThread(int rangeIndex, int start, int end)
        {
            List<int> primesInRange = new List<int>();
            if(start % 2 == 0)
            {
                start += 1;
            }
            for (int i = start; i <= end; i+=2)
            {
                int primeIndex = 1;
                bool isPrime = true;
                double limit = Math.Sqrt(i);
                while(primeIndex < primes.Count() && primes[primeIndex] <= limit)
                {
                    if (i % primes[primeIndex] == 0)
                    {
                        isPrime = false;
                        break;
                    }
                    primeIndex++;
                }

                if (isPrime)
                {
                    primesInRange.Add(i);
                }
            }

            newPrimes.Add(rangeIndex, primesInRange);
        }

        static void Main(string[] args)
        {
            //Multithread Branch
            
            var watch = System.Diagnostics.Stopwatch.StartNew();            

            int primesUpTo = int.MaxValue - 2;
            int lastPrimeCheck = 2;
            double sum = 0;

            for (int primeIndex = 1; primeIndex < primes.Count(); primeIndex++)
            {
                double limit = Math.Pow(primes[primeIndex], 2) - 1;
                if(limit > primesUpTo)
                {
                    limit = primesUpTo;
                }

                List<List<int>> primeRanges = new List<List<int>>();

                int valuesToCheck = (int)limit - lastPrimeCheck;
                int rangeCount = (valuesToCheck < THREAD_LIMIT) ? valuesToCheck : THREAD_LIMIT;
                int valuesPerRange = valuesToCheck / rangeCount;
                for(int i = 0; i < rangeCount; i++)
                {
                    int start = lastPrimeCheck + 1 + i * (valuesToCheck / rangeCount);
                    int end = (i == (rangeCount - 1)) ? (int)limit : (start + valuesPerRange - 1);
                    primeRanges.Add(new List<int>() { start, end });
                }

                List<Thread> threads = new List<Thread>();
                
                for(int primeRangeIndex = 0; primeRangeIndex < primeRanges.Count(); primeRangeIndex++)
                {
                    int tempIndex = primeRangeIndex;
                    List<int> range = primeRanges[tempIndex];
                    int start = range[0];
                    int end = range[1];
                    Thread tempThread = new Thread(() => primesInRangeThread(tempIndex, start, end));
                    threads.Add(tempThread);
                    tempThread.Start();
                }

                foreach(Thread thread in threads)
                {
                    thread.Join();
                }
                
                for(int key = 0; key < rangeCount; key++)
                {
                    if (newPrimes.ContainsKey(key))
                    {
                        primes.AddRange(newPrimes[key]);
                    }
                }

                newPrimes.Clear();
                lastPrimeCheck = (int)limit;

                if(limit == primesUpTo)
                {
                    break;
                }
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            foreach (int prime in primes)
            {
                sum += prime;
            }
            Console.WriteLine($"Number of primes from 1 to {primesUpTo}: {primes.Count()}");
            Console.WriteLine($"Sum of primes from 1 to {primesUpTo}: {sum}");
            Console.WriteLine($"Time to completion: {elapsedMs}ms");
        }
    }
}
