using MongoDB.Bson;
using System;


namespace BH.Adapter.Mongo
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BsonDocument ToBson(object obj, string tag, DateTime timestamp)
        {
            BsonDocument document = Engine.Serialiser.Convert.ToBson(obj);
            document["__Tag__"] = tag;
            document["__Time__"] = timestamp;

            return document;
        }

        /*******************************************/

        public static object FromBson(BsonDocument document)
        {
            document.Remove("__Tag__");
            document.Remove("__Time__");
            return Engine.Serialiser.Convert.FromBson(document);
        }

        /***************************************************/
    }
}
