using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mongo_Adapter;
using System.IO;

namespace Mongo_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test commit from Dummy - v1
            // Test commit 1
            // Test commit 2
            AutoStart(); 

            Console.Read();
        }

        static void AutoStart()
        {

            MongoServer server = new MongoServer(@"C:\Users\adecler\Documents\Mongo\Test_02");
            List<string> databases = server.GetAllDatabases();
        }

        static void OldTest()
        { 
            MongoLink link = new MongoLink(@"mongodb://risktool-user:Happold123!@ds040309.mlab.com:40309/risktool-bh", "risktool-bh", "projects");

            string findQuery = "{name: \"London\"}";

            List<string> query = new List<string>();
            query.Add("{ $match: {name: \"London\"} }");
            query.Add("{ $project: {name: 1, createdTime:1, lastChanged: 1} }");


            List<object> result = new List<object>();
            Console.WriteLine("Starting....");

            var watch = System.Diagnostics.Stopwatch.StartNew();
            result = link.Query(query).ToList();
            watch.Stop();
            Console.WriteLine("Aggregation query time: " + watch.ElapsedMilliseconds);

            watch = System.Diagnostics.Stopwatch.StartNew();
            result = link.Pull(findQuery).ToList();
            watch.Stop();
            Console.WriteLine("Find query time: " + watch.ElapsedMilliseconds);

            Console.WriteLine("Batch runs...");

            int nb = 10;

            var globalWatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < nb; i++)
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
                result = link.Pull(findQuery).ToList();
                watch.Stop();
                Console.WriteLine("Find query time: " + watch.ElapsedMilliseconds);
            }
            globalWatch.Stop();
            Console.WriteLine("Average Find query time: " + (globalWatch.ElapsedMilliseconds / nb));

            globalWatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < nb; i++)
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
                result = link.Query(query).ToList();
                watch.Stop();
                Console.WriteLine("Aggregation query time: " + watch.ElapsedMilliseconds);
            }
            globalWatch.Stop();
            Console.WriteLine("Total Aggregation query time: " + (globalWatch.ElapsedMilliseconds / nb));


            Console.Read();
        }
    }
}
