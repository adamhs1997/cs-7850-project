using System;
using System.Linq;
using PINQ;

namespace LINQAgain {
    class Program {

        // Obtaining the data source 
        static MedicalDataContext db = new MedicalDataContext();

        // Create the query 
        static IQueryable<BSOM_DataSet_revised> data = 
            from c in db.BSOM_DataSet_reviseds
            select c;

        // Get PINQ object--just wraps data queryable in PINQ Queryable
        static PINQueryable<BSOM_DataSet_revised> search =
            new PINQueryable<BSOM_DataSet_revised>(data, new PINQAgent());

        // Epsilon values we wish to test
        static double[] epsilons = { 10, 5, 1, 0.5, 0.1, 0.01 };

        // Number of iterations we wish to use for average calculations
        static int numIters = 100;

        static void Main(string[] args) {

            // Count
            TestWholeCount(data, search, epsilons);
            Console.WriteLine();

            // Average
            TestAverage(data, search, epsilons);
            Console.WriteLine();

            // Sum
            TestSum(data, search, epsilons);
            Console.WriteLine();

            // Ranged count
            TestRangedCount(data, search, epsilons);
            Console.WriteLine();

            // Generate average error on count over numIter iterations
            Console.WriteLine("Average error on counts...");
            double[] avgWholeCountError = 
                GenerateAverageErrors(GetErrorWholeCount);

            for (int i = 0; i < avgWholeCountError.Length; i++) {
                Console.WriteLine("Epsilon " + epsilons[i] + 
                    " avg error: " + avgWholeCountError[i]);
            }
            Console.WriteLine();

            // Generate average error on average over numIter iterations
            Console.WriteLine("Average error on averages...");
            double[] avgAverageError =
                GenerateAverageErrors(GetErrorAverage);

            for (int i = 0; i < avgWholeCountError.Length; i++) {
                Console.WriteLine("Epsilon " + epsilons[i] +
                    " avg error: " + avgAverageError[i]);
            }
            Console.WriteLine();

            // Generate average error on sum over numIter iterations
            Console.WriteLine("Average error on sums...");
            double[] avgSumError =
                GenerateAverageErrors(GetErrorAverage);

            for (int i = 0; i < avgWholeCountError.Length; i++) {
                Console.WriteLine("Epsilon " + epsilons[i] +
                    " avg error: " + avgSumError[i]);
            }
            Console.WriteLine();

            // Generate average error on sum over numIter iterations
            Console.WriteLine("Average error on ranged counts...");
            double[] avgRangeError =
                GenerateAverageErrors(GetErrorRangedCount);

            for (int i = 0; i < avgWholeCountError.Length; i++) {
                Console.WriteLine("Epsilon " + epsilons[i] +
                    " avg error: " + avgRangeError[i]);
            }
            Console.WriteLine();

            // Pause the application
            Console.ReadKey(true);
        }

        #region Basic query tests

        static void TestWholeCount(IQueryable<BSOM_DataSet_revised> data, 
          PINQueryable<BSOM_DataSet_revised> search, double[] epsilons) {
            Console.WriteLine("True Count: " + data.Count());

            foreach (double ep in epsilons) {
                Console.WriteLine(String.Format(
                    "Noisy Count (epsilon {0}): {1}", ep, 
                    search.NoisyCount(ep)));
            }
        }

        static void TestAverage(IQueryable<BSOM_DataSet_revised> data,
          PINQueryable<BSOM_DataSet_revised> search, double[] epsilons) {
            Console.WriteLine("True Average of O1_PI_01s: " + data.Average(
                x => Convert.ToDouble(x.O1_PI_01)));

            foreach (double ep in epsilons) {
                Console.WriteLine(String.Format(
                    "Noisy Average of O1_PI_01s (epsilon {0}): {1}", ep,
                    search.NoisyAverage(
                        ep, x => Convert.ToDouble(x.O1_PI_01))));
            }
        }

        static void TestSum(IQueryable<BSOM_DataSet_revised> data,
          PINQueryable<BSOM_DataSet_revised> search, double[] epsilons) {
            Console.WriteLine("True Sum of O1_PI_01s: " + data.Sum(
                x => Convert.ToDouble(x.O1_PI_01)));

            foreach (double ep in epsilons) {
                Console.WriteLine(String.Format(
                    "Noisy Average of O1_PI_01s (epsilon {0}): {1}", ep,
                    search.NoisySum(
                        ep, x => Convert.ToDouble(x.O1_PI_01))));
            }
        }

        static void TestRangedCount(IQueryable<BSOM_DataSet_revised> data,
          PINQueryable<BSOM_DataSet_revised> search, double[] epsilons) {
            Console.WriteLine("Count of Ranged Items: " + 
                data.Where(x => Convert.ToDouble(x.O1_PI_01) < 0.8).Count());

            foreach (double ep in epsilons) {
                Console.WriteLine(String.Format(
                    "Noisy Count of Ranged Items (epsilon {0}): {1}", ep,
                    search.Where(x => Convert.ToDouble(
                        x.O1_PI_01) < 0.8).NoisyCount(ep)));
            }
        }

        #endregion

        #region Average error calculations

        static double[] GenerateAverageErrors(Func<
          IQueryable<BSOM_DataSet_revised>, PINQueryable<BSOM_DataSet_revised>,
          double[], double[]> inFunc) {
            double[] avg = new double[epsilons.Length];
            for (int i = 0; i < numIters; i++) {
                double[] avgError = inFunc(data, search, epsilons);
                for (int j = 0; j < epsilons.Length; j++) {
                    avg[j] += avgError[j];
                }
            }

            for (int i = 0; i < avg.Length; i++) {
                avg[i] /= numIters;
            }

            return avg;
        }

        static double[] GetErrorWholeCount(IQueryable<BSOM_DataSet_revised> data,
          PINQueryable<BSOM_DataSet_revised> search, double[] epsilons) {
            // Get true value
            int trueCount = data.Count();

            // Get list to hold our answers
            double[] result = new double[epsilons.Length];

            // Calculate differences
            short idx = 0;
            foreach (double ep in epsilons) {
                result[idx++] = trueCount - search.NoisyCount(ep);
            }

            return result;
        }

        static double[] GetErrorAverage(IQueryable<BSOM_DataSet_revised> data,
          PINQueryable<BSOM_DataSet_revised> search, double[] epsilons) {
            // Get true value
            double trueAvg = data.Average(
                x => Convert.ToDouble(x.O1_PI_01));

            // Get list to hold our answers
            double[] result = new double[epsilons.Length];

            // Calculate differences
            short idx = 0;
            foreach (double ep in epsilons) {
                result[idx++] = trueAvg - search.NoisyAverage(
                    ep, x => Convert.ToDouble(x.O1_PI_01));
            }

            return result;
        }

        static double[] GetErrorSum(IQueryable<BSOM_DataSet_revised> data,
          PINQueryable<BSOM_DataSet_revised> search, double[] epsilons) {
            // Get true value
            double trueSum = data.Sum(
                x => Convert.ToDouble(x.O1_PI_01));

            // Get list to hold our answers
            double[] result = new double[epsilons.Length];

            // Calculate differences
            short idx = 0;
            foreach (double ep in epsilons) {
                result[idx++] = trueSum - search.NoisySum(
                    ep, x => Convert.ToDouble(x.O1_PI_01));
            }

            return result;
        }

        static double[] GetErrorRangedCount(IQueryable<BSOM_DataSet_revised> data,
          PINQueryable<BSOM_DataSet_revised> search, double[] epsilons) {
            // Get true value
            double trueSum = data.Where(
                x => Convert.ToDouble(x.O1_PI_01) < 0.8).Count();

            // Get list to hold our answers
            double[] result = new double[epsilons.Length];

            // Calculate differences
            short idx = 0;
            foreach (double ep in epsilons) {
                result[idx++] = trueSum - search.Where(x => Convert.ToDouble(
                    x.O1_PI_01) < 0.8).NoisyCount(ep);
            }

            return result;
        }

        #endregion
    }
}
