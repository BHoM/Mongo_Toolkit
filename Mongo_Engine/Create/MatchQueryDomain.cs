using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Mongo
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static List<string> MatchQueryDomain(List<object> Key, List<object> Upperbound, List<object> Lowerbound)
        {
            List<string> matchquery = new List<string>();
            string domainexpression,matchexpression = "";
            string tempvar = "";
            domainexpression = "{$addFields: {matchdomain: { $and: [{$gte: [" + BH.Engine.Mongo.Create.MongoCleanVariable(Key, tempvar) + "," + BH.Engine.Mongo.Create.MongoCleanVariable(Lowerbound, tempvar) +"]},{$lte: [" +BH.Engine.Mongo.Create.MongoCleanVariable(Key, tempvar) + "," + BH.Engine.Mongo.Create.MongoCleanVariable(Upperbound, tempvar) + "]}]}}}";
            matchexpression = "{$match: { matchdomain:true }}";
            matchquery.Add(domainexpression);
            matchquery.Add(matchexpression);
            return matchquery;

        }

    }
}
