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
        public static string MatchQueryDomain(object Key, List<object> Upperbound, List<object> Lowerbound)
        {
            string tempvar = "";
            string domainexpression = " { $match: { $and: [{" + Key+ ": { $gte:"+ BH.Engine.Mongo.Create.MongoCleanVariable(Lowerbound, tempvar)+" } }, { "+Key+": { $lte:"+ BH.Engine.Mongo.Create.MongoCleanVariable(Upperbound, tempvar)+ "} } ] } }";
            return domainexpression;
        }

    }
}
