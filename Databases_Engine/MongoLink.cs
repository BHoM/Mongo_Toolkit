using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Base;
using BHoM.Global;
using MongoDB.Bson;
using MongoDB.Driver;
using BHoM.Databases;
using System.IO;

namespace Mongo_Adapter
{
    public class MongoLink : IDatabaseAdapter
    {
        private MongoClient m_Client;
        private IMongoCollection<BsonDocument> m_Collection;

        public MongoLink(string serverLink = "mongodb://localhost:27017", string databaseName = "project", string collectionName = "bhomObjects")
        {
            m_Client = new MongoClient(serverLink);
            IMongoDatabase database = m_Client.GetDatabase(databaseName);
            m_Collection = database.GetCollection<BsonDocument>(collectionName);
        }

        /*******************************************/

        public string ServerName
        {
            get
            {
                MongoServerAddress server = m_Collection.Database.Client.Settings.Server;
                return "mongodb://" + server.ToString();
            }
        }

        /*******************************************/

        public string DatabaseName
        {
            get { return m_Collection.Database.DatabaseNamespace.DatabaseName;  }
        }

        /*******************************************/

        public string CollectionName
        {
            get { return m_Collection.CollectionNamespace.CollectionName;  }
        }

        /*******************************************/

        public bool Push(IEnumerable<object> objects, string key, List<string> tags = null)
        {
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return false;

            // Create the bulk query for the object to replace/insert
                List<WriteModel<BsonDocument>> bulk = new List<WriteModel<BsonDocument>>();
            bulk.Add(new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("__Key__", key)));
            foreach (object obj in objects)
                bulk.Add(new InsertOneModel<BsonDocument>(ToBson(obj, key)));

            // Send that query
            BulkWriteOptions bulkOptions = new BulkWriteOptions();
            bulkOptions.IsOrdered = true;
            m_Collection.BulkWrite(bulk, bulkOptions);
            return true;
        }

        /*******************************************/

        public bool Delete(string filterString = "{}")
        {
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return false;

            FilterDefinition<BsonDocument> filter = filterString;
            m_Collection.DeleteMany(filter);
            return true;
        }

        /*******************************************/

        public List<object> Pull(string filterString = "{}", bool keepAsString = false)
        {
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return new List<object>();

            FilterDefinition<BsonDocument> filter = filterString;
            List<BsonDocument> result = m_Collection.Find(filter).ToList();
            if (keepAsString)
                return result.Select(x => x.ToString()).ToList<object>();
            else
                return result.Select(x => FromBson(x)).ToList<object>();
        }

        /*******************************************/

        public List<object> Query(List<string> queryStrings = null, bool keepAsString = false)
        {
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return new List<object>();

            var pipeline = queryStrings.Select(s => BsonDocument.Parse(s)).ToList();
            List<BsonDocument> result = m_Collection.Aggregate<BsonDocument>(pipeline).ToList();
            if (keepAsString)
                return result.Select(x => x.ToString()).ToList<object>();
            else
                return result.Select(x => FromBson(x)).ToList<object>();
        }


        /*******************************************/
        /****  Private Helper Methods           ****/
        /*******************************************/

        private BsonDocument ToBson(object obj, string key)
        {
            if (obj is string) return ToBson(obj as string, key);
            var document = BsonDocument.Parse(BHoM.Base.JSONWriter.Write(obj));  
            if (key != "")
                document["__Key__"] = key;
            return document;
        }

        /*******************************************/

        private BsonDocument ToBson(string obj, string key)
        {
            var document = BsonDocument.Parse(obj);
            if (key != "")
                document["__Key__"] = key;
            return document;
        }

        /*******************************************/

        private object FromBson(BsonDocument bson)
        {
            MongoDB.Bson.IO.JsonWriterSettings writerSettings = new MongoDB.Bson.IO.JsonWriterSettings { OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict };
            return BHoM.Base.JsonReader.ReadObject(bson.ToJson(writerSettings));
        }

    }
}
