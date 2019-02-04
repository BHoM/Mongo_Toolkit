/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BH.oM.Base;
using BH.oM.Geometry;
using System.Threading;
using BH.oM.DataManipulation.Queries;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MongoDB.Bson;
using BH.Adapter.Mongo;
using MongoDB.Driver;

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
            //Test();

            TestConnection();

            Console.Read();

        }

        private static void TestConnection()
        {
            string s = "mongodb://BHoMTester:bhomtest1@ds018708.mlab.com:18708/bhom_test_status";

            //string s = "mongodb://BHoMTester:bhomtest1@ds018708.mlab.com:18708";

            MongoClient client = new MongoClient(s);

            Thread.Sleep(2000);

            bool connected = client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected;
        }

        public static void Test()
        {
            List<string> asdf = new List<string>();
            List<string> asdf2 = new List<string> {"Bar_Number", "Loadcase", "Force_Position"};
            List<string> asdf3 = new List<string> {"M_X","F_X" };
            List<string> g = new List<string>();
            //  g= BH.Engine.Mongo.Create.MongoCleanVariable(asdf, g);
            Console.WriteLine(g);
            List<object> fffff = new List<object> { g};
            // g = BH.Engine.Mongo.Create.MongoGroup(asdf2,asdf3);
            //     Console.WriteLine(g);
            g = BH.Engine.Mongo.Create.MongoMatchText("asdfasdf", asdf2);
                Console.WriteLine(g[0]);


        }


        public static void TestMongo()
        {


            /*List<object> items = new List<object> //TODO: Turn this into Custom BHoMObjects
            {
                new A (-6, -7) { a = 1, publicField = -4 },
                new B { a = 2, b = 45 },
                new C { a = 3, c = 56 },
                new D { a = 4, b = 67, d = 123, publicField = -4 },
                new E { a = 5, c = 78, e = 456 },
                new Dictionary<string, A> {
                    { "A",  new A { a = 1 } },
                    { "C",  new C { a = 3, c = 56 } },
                    { "E",  new E { a = 5, c = 78, e = 456 } }
                }
            };*/

            MongoAdapter link = new MongoAdapter();
            Thread.Sleep(1000);

            //link.Push(items, "tag");

            Thread.Sleep(1000);

            FilterQuery filter = new FilterQuery { Equalities = new Dictionary<string, object> { { "publicField", -4 } } };
            List<object> result = link.Pull(filter) as List<object>;
        }

        public static void TestBson()
        {

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

            List<BsonDocument> docs = items.Select(x => x.ToBsonDocument()).ToList();
            List<object> items2 = docs.Select(x => BsonSerializer.Deserialize(x, typeof(object))).ToList();

            foreach (BsonDocument doc in docs)
            {
                Console.WriteLine(doc.ToJson());
                Console.WriteLine();
            }

            string outputFileRoot = @"C:\Users\Arnaud\Documents\"; // initialize to the file to write to.
            File.WriteAllLines(@"C:\Users\Arnaud\Documents\json_Save.txt", docs.Select(x => x.ToJson()));

            FileStream mongoStream = new FileStream(outputFileRoot + "bsonSave_Mongo.txt", FileMode.Create);
            var writer = new BsonBinaryWriter(mongoStream);
            BsonSerializer.Serialize(writer, typeof(object), docs);
            mongoStream.Flush();
            mongoStream.Close();

            FileStream csharpStream = new FileStream(outputFileRoot + "bsonSave_CSharp.txt", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(csharpStream, docs);
            csharpStream.Close();

            FileStream mongoReadStream = File.OpenRead(outputFileRoot + "bsonSave_Mongo.txt");
            var reader = new BsonBinaryReader(mongoReadStream);
            List<BsonDocument> readBson = BsonSerializer.Deserialize(reader, typeof(object)) as List<BsonDocument>;
            List<object> items3 = readBson.Select(x => BsonSerializer.Deserialize(x, typeof(object))).ToList();

            // Directly writing and reading objects to the stream using Bson serializer seems to have a problem when reading back
            //FileStream objectStream = new FileStream(outputFileRoot + "objectSave_Mongo.txt", FileMode.Create);
            //var objectWriter = new BsonBinaryWriter(objectStream);
            //BsonSerializer.Serialize(objectWriter, typeof(List<object>), items);
            //objectStream.Flush();
            //objectStream.Close();

            //FileStream objectReadStream = File.OpenRead(outputFileRoot + "objectSave_Mongo.txt");
            //var objectReader = new BsonBinaryReader(objectReadStream);
            //var readObject = BsonSerializer.Deserialize(reader, typeof(object));


            Console.WriteLine("Done!");

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
