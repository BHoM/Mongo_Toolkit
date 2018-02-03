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
        public static string MatchQueryDomain(string Key, List<object> Upperbound, List<object> Lowerbound)
        {
            string matchquery = "";
            string tempvar = "";
            matchquery = "{$match: {"+Key+": {$gt:" +BH.Engine.Mongo.Create.MongoCleanVariable(Lowerbound, tempvar) + ",$lt:" + BH.Engine.Mongo.Create.MongoCleanVariable(Upperbound, tempvar) + "}}}";
            return matchquery;

        }

    }
}
