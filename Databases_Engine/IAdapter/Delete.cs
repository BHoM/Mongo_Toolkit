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
        public int Delete(FilterQuery filter, Dictionary<string, string> config = null)
        {
            DeleteResult result = m_Collection.DeleteMany(filter.ToMongoQuery());
            return (int)result.DeletedCount;
        }
        
    }
}
