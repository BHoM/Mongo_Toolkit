using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using BHC = BH.Adapter.Convert;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using BH.Adapter.Queries;
using BH.Adapter;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter : IAdapter
    {
        public int Update(FilterQuery filter, string property, object newValue, Dictionary<string, string> config = null)
        {
            BsonDocument definition = new BsonDocument();
            definition["__Time__"] = DateTime.Now;
            definition[property] = newValue.ToBson();

            UpdateResult result = m_Collection.UpdateMany(filter.ToMongoQuery(), definition);
            return (int)result.ModifiedCount;
        }

        public int UpdateProperty(FilterQuery filter, string property, object newValue, Dictionary<string, string> config = null)
        {
            return Update(filter, property, newValue, config);
        }

        public bool MoveToCollection(IEnumerable<IQuery> queries, MongoAdapter target, Dictionary<string, string> config = null)
        {
            IEnumerable<object> objects = this.Pull(queries, config);
            if (target.Push(objects))
            {
                //return this.Delete(filter) > 0;
                return true;
            }
            return false;
        }
    }
}
