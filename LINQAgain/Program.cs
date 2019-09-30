using System;
using System.Linq;
using PINQ;

namespace LINQAgain {
    class Program {
        static void Main(string[] args) {
            //Obtaining the data source 
            var db = new DataClasses1DataContext();

            // Create the query 
            var query = from c in db.BSOM_DataSet_reviseds
                        select c;
            var data = query.AsQueryable();

            #region LINQ version of the query

            // Execute the query
            Console.WriteLine("True Count: " + data.Count());

            #endregion

            #region PINQ version of the query

            // Get PINQ object--just wraps data queryable in PINQ Queryable
            PINQueryable<BSOM_DataSet_revised> search =
                new PINQueryable<BSOM_DataSet_revised>(data, new PINQAgent());

            // Run PINQ NoisyCount
            Console.WriteLine("Noisy Count: " + search.NoisyCount(10));
            Console.WriteLine("Noisy Count: " + search.NoisyCount(0.1));
            Console.WriteLine("Noisy Count: " + search.NoisyCount(1));
            Console.WriteLine("Noisy Count: " + search.NoisyCount(0.01));
            
            #endregion

            // Pause the application
            Console.ReadKey(true);
        }
    }
}
