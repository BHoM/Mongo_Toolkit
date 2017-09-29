using MongoDB.Bson;
using System;
using System.Collections.Generic;
using BHC = BH.Adapter.Convert;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BH.Adapter.Mongo
{
    public static partial class Convert
    {
        public static BsonDocument ToBson(object obj, string tag, DateTime timestamp)
        {
            BsonDocument document = BHC.ToBson(obj);
            document["__Tag__"] = tag;
            document["__Time__"] = timestamp;

            return document;
        }

        /*******************************************/

        public static object FromBson(BsonDocument document)
        {
            document.Remove("__Tag__");
            document.Remove("__Time__");
            return BHC.FromBson(document);
        }
    }
}
