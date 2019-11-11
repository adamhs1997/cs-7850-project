using System;
using System.Collections.Generic;
using System.Linq;
using MachineLearning;
using PINQ;

namespace LINQAgain {
    class KMeansTests {

        static void Main(string[] args) {
            // Get some data
            // Note that for this contrived dataset, we should almost always
            //  hit the desired centroids in 5 iterations
            double[][] rawData = GenerateSimpleData();
            
            // Get PINQ-safe data
            PINQueryable<double[]> pinqData = PackDataAsQueryable(rawData);

            // Do regular k-means
            Console.WriteLine("Ground-truth:");
            double[][] trueCentroids = RunPureKMeans(rawData, 2, 2, 5);

            // Do PINQ k-means, with several epsilons
            Console.WriteLine("PINQ:");
            double[] epsilons = { 100, 10, 5, 1, 0.5, 0.1, 0.01 };
            for (int i = 0; i < 5; i++) {
                foreach (double ep in epsilons) {
                    Console.WriteLine("Epsilon " + ep);
                    double[][] dpCentroids = RunPINQKMeans(pinqData, ep, 2, 2, 5);

                    // Calculate error distance for each centroid
                    Console.WriteLine("Error:");
                    foreach (double[] d in dpCentroids) {
                        Console.WriteLine(Math.Min(Dist(trueCentroids[0], d),
                                                   Dist(trueCentroids[1], d)));
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("\n---");

            // Try over some random data as well...
            for (int i = 0; i < 5; i++) {
                Console.WriteLine("Random Data, iteration " + (i + 1));

                // Get some data
                rawData = Generate2DRandomData();

                // Get PINQ-safe data
                pinqData = PackDataAsQueryable(rawData);

                // Do PINQ k-means
                RunPINQKMeans(pinqData, 100000, 2, 2, 10);
                RunPureKMeans(rawData, 2, 2, 10);
                Console.WriteLine();
            }

            // Pause the application
            Console.ReadKey(true);
        }

        static double[][] GenerateSimpleData() {
            // Return simple data with clear clusters to make sure
            //  algorithms work
            // Expected centroids: (0, 0), (0.85, 0.85)
            double[][] data = { new double[] { -0.2, -0.2 },
                                new double[] { 0, 0 },
                                new double[] { 0.2, 0.2 },
                                new double[] { 0.8, 0.8 },
                                new double[] { 0.9, 0.9 } };
            return data;
        }

        static double[][] Generate2DRandomData() {
            // Gets 1000 elements from the random number generator and
            //  returns them as queryable            
            return Program.GenerateData(2).Take(1000).ToArray();
        }

        static PINQueryable<double[]> PackDataAsQueryable(double[][] data) {
            // Wraps raw data with PINQueryable
            return new PINQueryable<double[]>(data.AsQueryable(), null);
        }

        static double[][] RunPINQKMeans(PINQueryable<double[]> data, double epsilon,
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

            return centroids;
        }

        static double[][] RunPureKMeans(double[][] data, int dataDims, int k, 
          int iters) {
            // Performs #iters toward forming #k clusters with
            //  k-means clustering, for #dataDims-dimensional data

            // Get random seed centroids
            double[][] centroids = Program.GenerateData(
                dataDims).Take(k).ToArray();

            // Run k times
            for (int iter = 0; iter < iters; iter++) {
                centroids = KMeansIter(data, centroids, dataDims);
            }

            // Print results
            Console.WriteLine("kMeans: {0} centers, {1} iterations", k, iters);
            foreach (double[] center in centroids) {
                foreach (double value in center)
                    Console.Write("\t{0:F4}", value);
                Console.WriteLine();
            }

            return centroids;
        }

        static double Dist(double[] v1, double[] v2) {
            // Used to find distance between two vectors
            double sum = 0;
            for (int i = 0; i < v1.Length; i++) {
                double d = v1[i] - v2[i];
                sum += d * d;
            }
            return Math.Sqrt(sum);
        }

        static double[][] KMeansIter(double[][] data, double[][] centroids,
          int dataDims) {
            // Get the nearest centroid to each data vector
            Dictionary<double[], List<double[]>> parts =
                new Dictionary<double[], List<double[]>>();
            foreach (double[] d in data) {
                double minDist = Double.MaxValue;
                double[] currentBest = null;

                foreach (double[] c in centroids) {
                    double dist = Dist(d, c);
                    if (dist < minDist) {
                        minDist = dist;
                        currentBest = c;
                    }
                }

                // Assign datapoint to centroid
                try {
                    parts[currentBest].Add(d);
                }
                catch (KeyNotFoundException) {
                    parts[currentBest] = new List<double[]> { d };
                }
            }

            // Update centers
            double[][] newCentroids = new double[centroids.Length][];
            int centroidNum = 0;
            foreach (var part in parts) {

                // Sum up each dimension of each datapoint
                double[] sums = new double[dataDims];
                foreach (double[] d in part.Value) {
                    for (int j = 0; j < dataDims; j++) {
                        sums[j] += d[j];
                    }
                }

                // Create new centroid
                for (int j = 0; j < dataDims; j++) {
                    sums[j] /= part.Value.Count;
                }
                newCentroids[centroidNum++] = sums;
            }

            // We may not have enough new centroids to fill the list
            //  Copy over the originals if so
            for (int i = 0; i < newCentroids.Length; i++)
                if (newCentroids[i] == null)
                    newCentroids[i] = new double[] { 0, 0 };

            return newCentroids;
        }
    }
}
