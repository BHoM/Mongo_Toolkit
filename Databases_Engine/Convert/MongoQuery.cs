using BH.Adapter.Queries;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.Mongo
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BsonDocument ToMongoQuery(this IQuery query)
        {
            return _ToMongoQuery(query as dynamic);
        }


        /***************************************************/
        /**** Private  Methods - Curves                 ****/
        /***************************************************/

        public static BsonDocument _ToMongoQuery(this CustomQuery query)
        {
            return BsonDocument.Parse(query.Query);
        }

        /***************************************************/

        public static BsonDocument _ToMongoQuery(this FilterQuery query)
        {
            BsonDocument document = new BsonDocument();

            // Define the match
            BsonDocument equalities = query.Equalities.ToBsonDocument();

            if (query.Type != null)
                equalities["_t"] = query.Type.ToString();
            if (query.Tag != "")
                equalities["__Tag__"] = query.Tag;

            document.Add(new BsonElement("$match", equalities));

            return document;
        }
    }
}
