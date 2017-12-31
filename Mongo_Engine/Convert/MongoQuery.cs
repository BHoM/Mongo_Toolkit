using BH.oM.Queries;
using MongoDB.Bson;

namespace BH.Adapter.Mongo
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Interface                           ****/
        /***************************************************/

        public static BsonDocument IToMongoQuery(this IQuery query)
        {
            return ToMongoQuery(query as dynamic);
        }


        /***************************************************/
        /**** Private  Methods - Curves                 ****/
        /***************************************************/

        public static BsonDocument ToMongoQuery(this CustomQuery query)
        {
            return BsonDocument.Parse(query.Query);
        }

        /***************************************************/

        public static BsonDocument ToMongoQuery(this FilterQuery query)
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

        /***************************************************/
    }
}
