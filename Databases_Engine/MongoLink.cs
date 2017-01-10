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

namespace Mongo_Adapter
{
    public class MongoLink : IDatabaseAdapter
    {
        private IMongoCollection<BsonDocument> collection;

        public MongoLink(string serverLink = "mongodb://localhost:27017", string databaseName = "project", string collectionName = "bhomObjects")
        {
            var mongo = new MongoClient(serverLink);
            IMongoDatabase database = mongo.GetDatabase(databaseName);
            collection = database.GetCollection<BsonDocument>(collectionName);
        }

        /*******************************************/

        public string ServerName
        {
            get
            {
                MongoServerAddress server = collection.Database.Client.Settings.Server;
                return "mongodb://" + server.ToString();
            }
        }

        /*******************************************/

        public string DatabaseName
        {
            get { return collection.Database.DatabaseNamespace.DatabaseName;  }
        }

        /*******************************************/

        public string CollectionName
        {
            get { return collection.CollectionNamespace.CollectionName;  }
        }

        /*******************************************/

        public bool Push(IEnumerable<object> objects, string key, List<string> tags = null)
        {
            // Create the bulk query for the object to replace/insert
            List<WriteModel<BsonDocument>> bulk = new List<WriteModel<BsonDocument>>();
            bulk.Add(new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("__Key__", key)));
            foreach (string obj in objects)
                bulk.Add(new InsertOneModel<BsonDocument>(ToBson(obj, key)));

            // Send that query
            BulkWriteOptions bulkOptions = new BulkWriteOptions();
            bulkOptions.IsOrdered = true;
            collection.BulkWrite(bulk, bulkOptions);
            return true;
        }

        /*******************************************/

        public bool Delete(string filterString = "{}")
        {
            FilterDefinition<BsonDocument> filter = filterString;
            collection.DeleteMany(filter);
            return true;
        }

        /*******************************************/

        public List<object> Pull(string filterString = "{}", bool keepAsString = false)
        {
            FilterDefinition<BsonDocument> filter = filterString;
            List<BsonDocument> result = collection.Find(filter).ToList();
            if (keepAsString)
                return result.Select(x => x.ToString()).ToList<object>();
            else
                return result.Select(x => FromBson(x)).ToList<object>();
        }

        /*******************************************/

        public List<object> Query(List<string> queryStrings = null, bool keepAsString = false)
        {
            var pipeline = queryStrings.Select(s => BsonDocument.Parse(s)).ToList();
            List<BsonDocument> result = collection.Aggregate<BsonDocument>(pipeline).ToList();
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
