using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using BHC = BHoM_Engine.DataStream.Convert;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using BH.Adapter.Queries;
using BH.Adapter;

namespace BH.Adapter.Mongo
{
    public class MongoAdapter : IAdapter
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public List<string> ErrorLog { get; set; } = new List<string>();

        public int HistorySize { get; set; } = 20;


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public MongoAdapter(string serverName = "mongodb://localhost:27017", string databaseName = "project", string collectionName = "bhomObjects")
        {
            if (!serverName.StartsWith("mongodb://"))
                serverName = "mongodb://" + serverName + ":27017";

            m_Client = new MongoClient(serverName);
            IMongoDatabase database = m_Client.GetDatabase(databaseName);
            m_Collection = database.GetCollection<BsonDocument>(collectionName);

            IMongoDatabase hist_Database = m_Client.GetDatabase(databaseName + "_History");
            m_History = hist_Database.GetCollection<BsonDocument>(collectionName);
        }


        /***************************************************/
        /**** Public Getter Methods                     ****/
        /***************************************************/

        public string GetServerName()
        {
            MongoServerAddress server = m_Collection.Database.Client.Settings.Server;
            return "mongodb://" + server.ToString();
        }

        /*******************************************/

        public string GetDatabaseName()
        {
            return m_Collection.Database.DatabaseNamespace.DatabaseName; 
        }

        /*******************************************/

        public string GetCollectionName()
        {
            return m_Collection.CollectionNamespace.CollectionName; 
        }


        /***************************************************/
        /**** Public CRUD Methods                       ****/
        /***************************************************/

        public bool Push(IEnumerable<object> objects, string key = "", Dictionary<string, string> config = null)
        {
            // Check that the link is still alive
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return false;

            // Get the config
            //string key = "";
            bool replace = true;

            if (config != null)
            {
                if (config.ContainsKey("Key"))
                    key = config["Key"];
                if (config.ContainsKey("Replace"))
                    bool.TryParse(config["Replace"], out replace);
            }

            // Create the bulk query for the object to replace/insert
            DateTime timestamp = DateTime.Now;
            IEnumerable<BsonDocument> documents = objects.Select(x => ToBson(x, key, timestamp));
            if (replace)
            {
                List<WriteModel<BsonDocument>> bulk = new List<WriteModel<BsonDocument>>();
                bulk.Add(new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("__Key__", key)));
                foreach (BsonDocument doc in documents)
                    bulk.Add(new InsertOneModel<BsonDocument>(doc));
                m_Collection.BulkWrite(bulk);
            }
            else
                m_Collection.InsertMany(documents);

            // Push in the history database as well
            List<IQuery> queries = new List<IQuery> {
                new CustomQuery("{$group: {_id: \"$__Time__\"}}"),
                new CustomQuery("{$sort: {_id: -1}}")
            };
            List<object> times = Pull(queries) as List<object>;
            if (times.Count > HistorySize)
                m_History.DeleteMany(Builders<BsonDocument>.Filter.Lte("__Time__", times[HistorySize]));
            m_History.InsertMany(documents);

            return true;
        }

        /***************************************************/

        public IList Pull(IEnumerable<IQuery> query, Dictionary<string, string> config = null)
        {
            // Check that the link is still alive
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return new List<object>();

            // Get the results
            var pipeline = query.Select(s => s.ToMongoQuery()).ToList();
            var aggregateOptions = new AggregateOptions() { AllowDiskUse = true };
            List<BsonDocument> result = m_Collection.Aggregate<BsonDocument>(pipeline, aggregateOptions).ToList();

            // Return as objects
            return result.Select(x => FromBson(x)).ToList<object>();
        }

        /***************************************************/

        public int Update(FilterQuery filter, Dictionary<string, object> changes, Dictionary<string, string> config = null)
        {
            DateTime timestamp = DateTime.Now;
            UpdateResult result = m_Collection.UpdateMany(filter.ToMongoQuery(), ToBson(changes, timestamp));
            return (int)result.ModifiedCount;
        }


        /***************************************************/

        public int Delete(FilterQuery filter, Dictionary<string, string> config = null)
        {
            DeleteResult result = m_Collection.DeleteMany(filter.ToMongoQuery());
            return (int)result.DeletedCount;
        }

        /***************************************************/

        public bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, string> config = null)
        {
            switch (command)
            {
                case "Transfer":
                    if (parameters != null && parameters.ContainsKey("Destination"))
                    {
                        bool replaceContent = false;
                        if (config != null && config.ContainsKey("Replace"))
                            bool.TryParse(config["Replace"], out replaceContent);
                        return MoveCollection(parameters["Destination"] as MongoAdapter, replaceContent);
                    }
                    break;
            }

            throw new NotImplementedException(command + " is not a recognised command for the Mongo adapter.");
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private MongoClient m_Client;
        private IMongoCollection<BsonDocument> m_Collection;
        private IMongoCollection<BsonDocument> m_History;


        /*******************************************/
        /****  Private Bson Methods             ****/
        /*******************************************/

        private BsonDocument ToBson(object obj, string key, DateTime timestamp)
        {
            var document = BHC.Bson.Write(obj);
            document["__Key__"] = key;
            document["__Time__"] = timestamp;

            return document;
        }

        /*******************************************/

        private BsonDocument ToBson(Dictionary<string, object> definition, DateTime timestamp)
        {
            definition["__Time__"] = timestamp;
            return new BsonDocument(definition);
        }

        /*******************************************/

        private object FromBson(BsonDocument document)
        {
            document.Remove("__Key__");
            document.Remove("__Time__");
            return BHC.Bson.Read(document);
        }


        /*******************************************/
        /****  Private Execute Methods          ****/
        /*******************************************/

        public bool MoveCollection(MongoAdapter other, bool replaceContent = true)
        {
            try
            {
                //Access the admin namespace and admin db needed to be able to rename collections
                var adminDBNameSpace = DatabaseNamespace.Admin;
                var adminDb = m_Client.GetDatabase(adminDBNameSpace.DatabaseName);

                //Create the renaming command
                Command<BsonDocument> command = "{ renameCollection: \"" +
                                                this.GetDatabaseName() + "." + this.GetCollectionName() +
                                                "\", to:\"" +
                                                other.GetDatabaseName() + "." + other.GetCollectionName() +
                                                "\", dropTarget:\"" + replaceContent.ToString() + "\"}";

                //Execute command
                adminDb.RunCommand(command);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /*******************************************/

        /*public List<string> GetHistoryTimes()
        {
            List<object> times = Query(new List<string> { "{$group: {_id: \"$__Time__\"}}", "{$sort: {_id: -1}}" });

        }*/



        /*******************************************/

        //private BsonDocument ToBson(string obj, string key, DateTime timestamp)
        //{
        //    var document = BsonDocument.Parse(obj);
        //    if (key != "")
        //    {
        //        document["__Key__"] = key;
        //        document["__Time__"] = timestamp;
        //    }

        //    return document;
        //}

        /*******************************************/

        //private object FromBson(BsonDocument bson)
        //{
        //    MongoDB.Bson.IO.JsonWriterSettings writerSettings = new MongoDB.Bson.IO.JsonWriterSettings { OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict };
        //    return BHoM.Base.JsonReader.ReadObject(bson.ToJson(writerSettings));
        //}

    }
}
