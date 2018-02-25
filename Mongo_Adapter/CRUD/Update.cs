using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using BH.oM.DataManipulation.Queries;
using BH.Engine.Mongo;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override int UpdateProperty(FilterQuery filter, string property, object newValue, Dictionary<string, object> config = null)
        {
            BsonDocument definition = new BsonDocument();
            definition["__Time__"] = DateTime.Now;
            definition[property] = newValue.ToBson();

            UpdateResult result = m_Collection.UpdateMany(filter.ToMongoQuery(), definition);
            return (int)result.ModifiedCount;
        }

        /***************************************************/
    }
}
