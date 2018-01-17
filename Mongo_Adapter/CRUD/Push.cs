using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using BH.oM.Queries;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override bool Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            // Check that the link is still alive
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return false;

            // Get the config
            bool replace = true;
            if (config != null)
            {
                if (config.ContainsKey("Tag"))
                    tag = config["Tag"] as string;
                if (config.ContainsKey("Replace"))
                    bool.TryParse(config["Replace"] as string, out replace);
            }

            // Create the bulk query for the object to replace/insert
            DateTime timestamp = DateTime.Now;
            IEnumerable<BsonDocument> documents = objects.Select(x => Convert.ToBson(x, tag, timestamp));
            if (replace)
            {
                List<WriteModel<BsonDocument>> bulk = new List<WriteModel<BsonDocument>>();
                bulk.Add(new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("__Tag__", tag)));
                foreach (BsonDocument doc in documents)
                    bulk.Add(new InsertOneModel<BsonDocument>(doc));
                m_Collection.BulkWrite(bulk);
            }
            else
                m_Collection.InsertMany(documents);

            // Push in the history database as well
            BatchQuery queries = new BatchQuery(new List<IQuery> {
                new CustomQuery("{$group: {_id: \"$__Time__\"}}"),
                new CustomQuery("{$sort: {_id: -1}}")
            });
            List<object> times = Pull(queries) as List<object>;
            if (times.Count > HistorySize)
                m_History.DeleteMany(Builders<BsonDocument>.Filter.Lte("__Time__", times[HistorySize]));
            m_History.InsertMany(documents);

            return true;
        }

        /***************************************************/
    }
}
