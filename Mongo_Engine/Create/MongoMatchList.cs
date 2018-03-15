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
        public static List<string> MongoMatchList(string Key, List<object> Filter)
        {
            string projectquery = "";
            string matchquery = "";
            string mongolist = "";
            string tempVar = "";
            List<string> aggregatecommand = new List<string>();
            mongolist =  BH.Engine.Mongo.Create.MongoCleanVariable(Filter,tempVar);
  
             projectquery = "{$addFields: {isinfilterlist_" + Key + " : {$in: [" + "\"$" + Key + "\" , " + "[" + mongolist + "]] } } }"; 
             matchquery= "{$match: { " + "isinfilterlist_" + Key  + " : true} }";

            aggregatecommand.Add(projectquery);
            aggregatecommand.Add(matchquery);

            return aggregatecommand;
            
        }

    }
}
