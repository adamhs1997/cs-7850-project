using System;
using System.Linq;
using MachineLearning;
using PINQ;

namespace LINQAgain {
    class KMeansTests {

        static void Main(string[] args) {
            // Get some data
            IQueryable<double[]> rawData = GenerateSimpleData();// Generate2DRandomData();

            // Get PINQ-safe data
            PINQueryable<double[]> pinqData = GeneratePINQData(rawData);

            // Do PINQ k-means
            RunPINQKMeans(pinqData, 1000, 2, 2, 5);

            // Pause the application
            Console.ReadKey(true);
        }

        static IQueryable<double[]> GenerateSimpleData() {
            // Return simple data with clear clusters to make sure
            //  algorithms work
            // Expected centroids: 0, 0.85
            double[][] data = { new double[] { -0.2, -0.2 },
                                new double[] { 0, 0 },
                                new double[] { 0.2, 0.2 },
                                new double[] { 0.8, 0.8 },
                                new double[] { 0.9, 0.9 } };
            return data.AsQueryable();
        }

        static IQueryable<double[]> Generate2DRandomData() {
            // Gets 1000 elements from the random number generator and
            //  returns them as queryable            
            return Program.GenerateData(2).Take(1000).ToArray().AsQueryable();
        }

        static PINQueryable<double[]> GeneratePINQData(
          IQueryable<double[]> data) {
            // Wraps raw queryable object with PINQueryable
            return new PINQueryable<double[]>(data, null);
        }

        static void RunPINQKMeans(PINQueryable<double[]> data, double epsilon,
          int dataDims, int k, int iters) {
            // Performs #iters toward forming #k clusters with PINQ-DP
            //  k-means clustering, for #dataDims-dimensional data
            // Exact implementation from PINQ MachineLearning example

            // Get random seed centroids
            double[][] centroids = Program.GenerateData(
                dataDims).Take(k).ToArray();

            // Do some k-means!
            for (int i = 0; i < iters; i++) {
                Program.kMeansStep(data, centroids, epsilon);
            }

            // Print results
            Console.WriteLine("kMeans: {0} centers, {1} iterations", k, iters);
            foreach (double[] center in centroids) {
                foreach (double value in center)
                    Console.Write("\t{0:F4}", value);
                Console.WriteLine();
            }
        }
    }
}
