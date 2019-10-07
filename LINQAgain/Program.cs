using System;
using System.Linq;
using PINQ;

namespace LINQAgain {
    class Program {
        static void Main(string[] args) {
            // Obtaining the data source 
            var db = new MedicalDataContext();

            // Create the query 
            var query = from c in db.BSOM_DataSet_reviseds
                        select c;
            var data = query.AsQueryable();

            #region LINQ version of the query

            // LINQ results
            Console.WriteLine("LINQ RESULTS");

            // Count
            Console.WriteLine("True Count: " + data.Count());

            // Average
            Console.WriteLine("True Average of O1_PI_01s: " + data.Average(
                x => Convert.ToDouble(x.O1_PI_01)));

            // Sum
            Console.WriteLine("True Sum of O1_PI_01s: " + data.Sum(
                x => Convert.ToDouble(x.O1_PI_01)));

            // Median
            // NOTE: This is not natively supported in LINQ as it is in PINQ,
            //   so we must compute ourselves
            // REFERENCE (modified from): 
            //   https://blogs.msmvps.com/deborahk/linq-mean-median-and-mode/
            int numberCount = data.Count();
            int halfIndex = numberCount / 2;
            var sortedNumbers = data.OrderBy(
                x => Convert.ToDouble(x.O1_PI_01));
            double median;
            if ((numberCount % 2) == 0) {
                median = ((Convert.ToDouble(sortedNumbers.Skip(
                    halfIndex).First().O1_PI_01) + Convert.ToDouble(
                    sortedNumbers.Skip(halfIndex - 1).First().O1_PI_01)) / 2);
            }
            else {
                median = Convert.ToDouble(sortedNumbers.Skip(
                    halfIndex).First().O1_PI_01);
            }
            Console.WriteLine("True Median of O1_PI_01s: " + median);

            #endregion

            #region PINQ version of the query

            // Get PINQ object--just wraps data queryable in PINQ Queryable
            PINQueryable<BSOM_DataSet_revised> search =
                new PINQueryable<BSOM_DataSet_revised>(data, new PINQAgent());

            // Run PINQ NoisyCount
            Console.WriteLine("\nPINQ RESULTS");
            Console.WriteLine("Noisy Count (epsilon 10): " + 
                search.NoisyCount(10));
            Console.WriteLine("Noisy Count (epsilon 0.1): " + 
                search.NoisyCount(0.1));
            Console.WriteLine("Noisy Count (epsilon 1): " + 
                search.NoisyCount(1));
            Console.WriteLine("Noisy Count (epsilon 0.01): " + 
                search.NoisyCount(0.01));

            // Run PINQ NoisyAverage
            Console.WriteLine("Noisy Average of O1_PI_01s (epsilon 10): " + 
                search.NoisyAverage(10, x => Convert.ToDouble(x.O1_PI_01)));
            Console.WriteLine("Noisy Average of O1_PI_01s (epsilon 0.1): " +
                search.NoisyAverage(0.1, x => Convert.ToDouble(x.O1_PI_01)));
            Console.WriteLine("Noisy Average of O1_PI_01s (epsilon 1): " +
                search.NoisyAverage(1, x => Convert.ToDouble(x.O1_PI_01)));
            Console.WriteLine("Noisy Average of O1_PI_01s (epsilon 0.01): " +
                search.NoisyAverage(0.01, x => Convert.ToDouble(x.O1_PI_01)));

            // Run PINQ NoisySum
            Console.WriteLine("Noisy Sum of O1_PI_01s (epsilon 10): " +
                search.NoisySum(10, x => Convert.ToDouble(x.O1_PI_01)));
            Console.WriteLine("Noisy Sum of O1_PI_01s (epsilon 0.1): " +
                search.NoisySum(0.1, x => Convert.ToDouble(x.O1_PI_01)));
            Console.WriteLine("Noisy Sum of O1_PI_01s (epsilon 1): " +
                search.NoisySum(1, x => Convert.ToDouble(x.O1_PI_01)));
            Console.WriteLine("Noisy Sum of O1_PI_01s (epsilon 0.01): " +
                search.NoisySum(0.01, x => Convert.ToDouble(x.O1_PI_01)));

            // Run PINQ NoisyMedian
            Console.WriteLine("Noisy Median of O1_PI_01s (epsilon 10): " +
                search.NoisyMedian(10, x => Convert.ToDouble(x.O1_PI_01)));
            Console.WriteLine("Noisy Median of O1_PI_01s (epsilon 0.1): " +
                search.NoisyMedian(0.1, x => Convert.ToDouble(x.O1_PI_01)));
            Console.WriteLine("Noisy Median of O1_PI_01s (epsilon 1): " +
                search.NoisyMedian(1, x => Convert.ToDouble(x.O1_PI_01)));
            Console.WriteLine("Noisy Median of O1_PI_01s (epsilon 0.01): " +
                search.NoisyMedian(0.01, x => Convert.ToDouble(x.O1_PI_01)));

            #endregion

            // Pause the application
            Console.ReadKey(true);
        }
    }
}
