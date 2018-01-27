using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using BH.oM.Queries;
using BH.Engine.Mongo;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override IEnumerable<object> Pull(IQuery query, Dictionary<string, object> config = null)
        {
            // Check that the link is still alive
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return new List<object>();

            // Get the results
            List<BsonDocument> pipeline = new List<BsonDocument>();
            if (query is BatchQuery)
                pipeline = ((BatchQuery)query).Queries.Select(s => s.IToMongoQuery()).ToList();
            else
                pipeline.Add(query.IToMongoQuery());
            var aggregateOptions = new AggregateOptions() { AllowDiskUse = true };
            List<BsonDocument> result = m_Collection.Aggregate<BsonDocument>(pipeline, aggregateOptions).ToList();

            // Return as objects
            return result.Select(x => Engine.Mongo.Convert.FromBson(x)).ToList<object>();
        }


        /***************************************************/
    }
}
