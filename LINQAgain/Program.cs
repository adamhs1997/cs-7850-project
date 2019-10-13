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

            // Epsilon values we wish to test
            double[] epsilons = { 10, 5, 1, 0.5, 0.1, 0.01 };

            // Get PINQ object--just wraps data queryable in PINQ Queryable
            PINQueryable<BSOM_DataSet_revised> search =
                new PINQueryable<BSOM_DataSet_revised>(data, new PINQAgent());

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

            // Pause the application
            Console.ReadKey(true);
        }

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
    }
}
