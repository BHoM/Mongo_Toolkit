using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using BH.Adapter.Queries;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter 
    {
        public override int Delete(FilterQuery filter, Dictionary<string, string> config = null)
        {
            DeleteResult result = m_Collection.DeleteMany(filter.ToMongoQuery());
            return (int)result.DeletedCount;
        }
        
    }
}
