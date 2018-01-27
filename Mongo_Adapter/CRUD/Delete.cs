using System.Collections.Generic;
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

        public override int Delete(FilterQuery filter, Dictionary<string, object> config = null)
        {
            DeleteResult result = m_Collection.DeleteMany(filter.ToMongoQuery());
            return (int)result.DeletedCount;
        }


        /***************************************************/
    }
}
