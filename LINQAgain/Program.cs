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

        // Number of iterations we wish to use for calculations
        static int numIters = 100;

        static void Main(string[] args) {
            // Run basic query tests, just to ensure everything works
            //RunBasicTests();

            // Run tests on transformed data, seeing (1) how the results
            //  compare, and (2) when we run out of privacy budget
            RunTransformativeTests();

            // Calculate the average error on different queries
            //  across several iterations
            //RunAverageCalculations();

            // Calculate the variance of errors on different queries
            //  across several iterations
            //RunVarianceCalculations();

            // Pause the application
            Console.ReadKey(true);
        }

        #region Driver function calls
        
        static void RunBasicTests() {
            // Count
            TestWholeCount(data, search, epsilons);
            Console.WriteLine();

            // Average
            TestAverage(data, search, epsilons);
            Console.WriteLine();

            // Sum
            TestSum(data, search, epsilons);
            Console.WriteLine();
        }

        static void RunTransformativeTests() {
            // Ranged count
            TestWhere(data, search, epsilons);
            Console.WriteLine();

            // Group by
            TestGroupBy(data, search, epsilons);
            Console.WriteLine();

            // Group by with select
            TestGroupByWhere(data, search, epsilons);
            Console.WriteLine();
        }

        static void RunAverageCalculations() {
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
                GenerateAverageErrors(GetErrorSum);

            for (int i = 0; i < avgWholeCountError.Length; i++) {
                Console.WriteLine("Epsilon " + epsilons[i] +
                    " avg error: " + avgSumError[i]);
            }
            Console.WriteLine();

            // Generate average error on ranged count over numIter iterations
            Console.WriteLine("Average error on ranged counts...");
            double[] avgRangeError =
                GenerateAverageErrors(GetErrorRangedCount);

            for (int i = 0; i < avgWholeCountError.Length; i++) {
                Console.WriteLine("Epsilon " + epsilons[i] +
                    " avg error: " + avgRangeError[i]);
            }
            Console.WriteLine();
        }

        static void RunVarianceCalculations() {
            // Generate variance on count over numIter iterations
            Console.WriteLine("Variance of counts...");
            double[][] error = GenerateRangeError(GetErrorWholeCount);

            for (int i = 0; i < error[0].Length; i++) {
                Console.WriteLine(String.Format(
                    "Epsilon {0} min variance: {1}; max variance: {2}",
                    epsilons[i], error[0][i], error[1][i]));
            }
            Console.WriteLine();

            // Generate variance on average over numIter iterations
            Console.WriteLine("Variance of averages...");
            error = GenerateRangeError(GetErrorAverage);

            for (int i = 0; i < error[0].Length; i++) {
                Console.WriteLine(String.Format(
                    "Epsilon {0} min variance: {1}; max variance: {2}",
                    epsilons[i], error[0][i], error[1][i]));
            }
            Console.WriteLine();

            // Generate variance on sum over numIter iterations
            Console.WriteLine("Variance of sums...");
            error = GenerateRangeError(GetErrorSum);

            for (int i = 0; i < error[0].Length; i++) {
                Console.WriteLine(String.Format(
                    "Epsilon {0} min variance: {1}; max variance: {2}",
                    epsilons[i], error[0][i], error[1][i]));
            }
            Console.WriteLine();

            // Generate variance on ranged count over numIter iterations
            Console.WriteLine("Variance of ranged counts...");
            error = GenerateRangeError(GetErrorRangedCount);

            for (int i = 0; i < error[0].Length; i++) {
                Console.WriteLine(String.Format(
                    "Epsilon {0} min variance: {1}; max variance: {2}",
                    epsilons[i], error[0][i], error[1][i]));
            }
            Console.WriteLine();
        }

        #endregion

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

        #endregion

        #region Tranformative query tests

        static void TestWhere(IQueryable<BSOM_DataSet_revised> data,
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

        static void TestGroupBy(IQueryable<BSOM_DataSet_revised> data,
          PINQueryable<BSOM_DataSet_revised> search, double[] epsilons) {
            // We are grouping IDs by their O1_PI_01 scores
            Console.WriteLine("Count of distinct O1_PI_01 score groups: " +
                data.GroupBy(x => x.O1_PI_01).Count());

            foreach (double ep in epsilons) {
                Console.WriteLine("Noisy count of distinct O1_PI_01 score groups " +
                    "(epsilon " + ep + "): " +
                    search.GroupBy(x => x.O1_PI_01).NoisyCount(ep));
            }
        }

        static void TestGroupByWhere(IQueryable<BSOM_DataSet_revised> data,
          PINQueryable<BSOM_DataSet_revised> search, double[] epsilons) {
            // Group IDs by O1_PI_01 scores, then select those that have
            //  more than 5 in each group
            var result = data.GroupBy(x => x.O1_PI_01).Select(
              group => new {
                key = group.Key,
                count = group.Count()
              }).Where(group => group.count > 5);
            Console.WriteLine("Count of distinct O1_PI_01 score groups" +
                " with more than 5 members: " + result.Count());

            var pinqResult = search.GroupBy(x => x.O1_PI_01).Select(
              group => new {
                key = group.Key,
                count = group.Count()
              }).Where(group => group.count > 5);
            foreach (double ep in epsilons) {
                Console.WriteLine("Noisy count (epsilon " + ep + "): " + 
                    pinqResult.NoisyCount(ep));
            }
        }

        #endregion

        #region Cumulative error calculations

        static double[][] GenerateRangeError(Func<
          IQueryable<BSOM_DataSet_revised>, PINQueryable<BSOM_DataSet_revised>,
          double[], double[]> inFunc) {
            double[] minRanges = new double[epsilons.Length];
            double[] maxRanges = new double[epsilons.Length];

            // Initialize our min/max value arrays
            for (int i = 0; i < epsilons.Length; i++) {
                minRanges[i] = Double.MaxValue;
                maxRanges[i] = Double.MinValue;
            }

            // Find our min and max variances between true and noisy
            //  answer for each epsilon value
            for (int i = 0; i < numIters; i++) {
                // Get errors
                double[] errors = inFunc(data, search, epsilons);

                // Assign min/max errors accordingly
                for (int j = 0; j < epsilons.Length; j++) {
                    double absError = Math.Abs(errors[j]);
                    if (absError < minRanges[j])
                        minRanges[j] = absError;
                    if (absError > maxRanges[j])
                        maxRanges[j] = absError;
                }
            }

            return new double[][] { minRanges, maxRanges };
        }

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
