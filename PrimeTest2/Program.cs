using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PrimeTest2
{
    class Program
    {
        //Contains primes up to specified limit, initialized with first two primes
        static public List<int> primes = new List<int> { 1, 2 };

        //Contains primes found by threads
        static public Dictionary<int, List<int>> newPrimes = new Dictionary<int, List<int>>();

        //Max number of threads we can run concurrently
        static public int THREAD_LIMIT = Environment.ProcessorCount;

        //Checks primality of numbers in a range and populates newPrimes
        static public void primesInRangeThread(int rangeIndex, int start, int end)
        {
            List<int> primesInRange = new List<int>();

            //Make sure we start on an odd number
            if(start % 2 == 0)
            {
                start += 1;
            }

            //For each odd number in the range
            for (int i = start; i <= end; i+=2)
            {
                //Start out checking modulo 2
                int primeIndex = 1;

                //Assume primality
                bool isPrime = true;

                //Don't need to check for remainder on primes greater than sqrt of i
                double limit = Math.Sqrt(i);

                //Check remainder on all primes less than or equal to sqrt(i)
                while(primeIndex < primes.Count() && primes[primeIndex] <= limit)
                {
                    //i isn't prime
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
            var watch = System.Diagnostics.Stopwatch.StartNew();

            //Ceiling for primes
            int primesUpTo = 1000000000;

            //Last int checked for primality
            int lastPrimeCheck = 2;

            int lastPercentUpdate = 0;

            //For each prime in primes other than 1
            for (int primeIndex = 1; primeIndex < primes.Count(); primeIndex++)
            {
                //Ceiling of the values that can be checked with the primes up to primes[primeIndex]
                double limit = Math.Pow(primes[primeIndex], 2) - 1;

                //If limit exceeds the ceiling from primesUpTo, we make that the ceiling
                if(limit > primesUpTo)
                {
                    limit = primesUpTo;
                }

                //Contains the ranges each thread will check for prime numbers
                List<List<int>> primeRanges = new List<List<int>>();

                int valuesToCheck = (int)limit - lastPrimeCheck;

                //Number of ranges, each gets its own thread
                int rangeCount = (valuesToCheck < THREAD_LIMIT) ? valuesToCheck : THREAD_LIMIT;
                int valuesPerRange = valuesToCheck / rangeCount;

                //Populate ranges
                for(int i = 0; i < rangeCount; i++)
                {
                    int start = lastPrimeCheck + 1 + i * (valuesToCheck / rangeCount);
                    int end = (i == (rangeCount - 1)) ? (int)limit : (start + valuesPerRange - 1);
                    primeRanges.Add(new List<int>() { start, end });
                }

                List<Thread> threads = new List<Thread>();
                
                //Create and start threads
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

                //Wait for all threads to finish checking their ranges
                foreach(Thread thread in threads)
                {
                    thread.Join();
                }
                
                //Add new primes to existing primes list
                for(int key = 0; key < rangeCount; key++)
                {
                    if (newPrimes.ContainsKey(key))
                    {
                        primes.AddRange(newPrimes[key]);
                    }
                }

                //Clear the dictionary
                newPrimes.Clear();

                //Break from the loop if we've reached our ceiling
                if(limit == primesUpTo)
                {
                    Console.WriteLine("Progress: 100%");
                    break;
                }
                else
                {
                    int percent = (int) ((double)primes[primeIndex] / Math.Sqrt(primesUpTo) * 100);
                    if(percent != lastPercentUpdate)
                    {
                        Console.WriteLine($"Progress: {percent}%, {primes.Count()} primes found");
                        lastPercentUpdate = percent;
                    }
                }

                //Update last prime checked to limit
                lastPrimeCheck = (int)limit;
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            double sum = 0;
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
