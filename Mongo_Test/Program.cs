﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mongo_Adapter;
using System.IO;
using BH.oM.Structural.Elements;
using BH.oM.Base;
using BH.oM.Geometry;
using System.Threading;

namespace Mongo_Test
{
    class A
    {

        public A(double gVal = 0, double pVal = 0)
        {
            getProp = gVal;
            privateField = pVal;
        }

        public double a { get; set; }
        public double publicField;
        private double privateField;
        public double getProp { get; private set; }
    }
    class B : A { public double b { get; set; } }
    class C : A { public double c { get; set; } }
    class D : B { public double d { get; set; } }
    class E : C { public double e { get; set; } }

    class Program
    {
        static void Main(string[] args)
        {
            List<BHoMObject> nodes = new List<BHoMObject>
            {
                new Node {Point = new Point(1, 2, 3), Name = "A"},
                new Node {Point = new Point(4, 5, 6), Name = "B"},
                new Node {Point = new Point(7, 8, 9), Name = "C"}
            };

            List<object> items = new List<object>
            {
                new A (-6, -7) { a = 1, publicField = -4 },
                new B { a = 2, b = 45 },
                new C { a = 3, c = 56 },
                new D { a = 4, b = 67, d = 123 },
                new E { a = 5, c = 78, e = 456 },
                new Dictionary<string, A> {
                    { "A",  new A { a = 1 } },
                    { "C",  new C { a = 3, c = 56 } },
                    { "E",  new E { a = 5, c = 78, e = 456 } }
                }
            };

            MongoLink link = new MongoLink();
            link.Push(items, "key");

            Thread.Sleep(1000);

            List<object> result = link.Pull(new string[] { "{$match: {}}" }) as List<object>;

            Console.Read();
        }

        //static void AutoStart()
        //{

        //    MongoServer server = new MongoServer(@"C:\Users\adecler\Documents\Mongo\Test_02");
        //    List<string> databases = server.GetAllDatabases();
        //}

        //static void OldTest()
        //{ 
        //    MongoLink link = new MongoLink(@"mongodb://risktool-user:Happold123!@ds040309.mlab.com:40309/risktool-bh", "risktool-bh", "projects");

        //    string findQuery = "{name: \"London\"}";

        //    List<string> query = new List<string>();
        //    query.Add("{ $match: {name: \"London\"} }");
        //    query.Add("{ $project: {name: 1, createdTime:1, lastChanged: 1} }");


        //    List<object> result = new List<object>();
        //    Console.WriteLine("Starting....");

        //    var watch = System.Diagnostics.Stopwatch.StartNew();
        //    result = link.Query(query).ToList();
        //    watch.Stop();
        //    Console.WriteLine("Aggregation query time: " + watch.ElapsedMilliseconds);

        //    watch = System.Diagnostics.Stopwatch.StartNew();
        //    result = link.Pull(findQuery).ToList();
        //    watch.Stop();
        //    Console.WriteLine("Find query time: " + watch.ElapsedMilliseconds);

        //    Console.WriteLine("Batch runs...");

        //    int nb = 10;

        //    var globalWatch = System.Diagnostics.Stopwatch.StartNew();
        //    for (int i = 0; i < nb; i++)
        //    {
        //        watch = System.Diagnostics.Stopwatch.StartNew();
        //        result = link.Pull(findQuery).ToList();
        //        watch.Stop();
        //        Console.WriteLine("Find query time: " + watch.ElapsedMilliseconds);
        //    }
        //    globalWatch.Stop();
        //    Console.WriteLine("Average Find query time: " + (globalWatch.ElapsedMilliseconds / nb));

        //    globalWatch = System.Diagnostics.Stopwatch.StartNew();
        //    for (int i = 0; i < nb; i++)
        //    {
        //        watch = System.Diagnostics.Stopwatch.StartNew();
        //        result = link.Query(query).ToList();
        //        watch.Stop();
        //        Console.WriteLine("Aggregation query time: " + watch.ElapsedMilliseconds);
        //    }
        //    globalWatch.Stop();
        //    Console.WriteLine("Total Aggregation query time: " + (globalWatch.ElapsedMilliseconds / nb));


        //    Console.Read();
        //}
    }
}
