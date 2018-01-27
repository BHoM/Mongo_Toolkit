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
        public static List<string> MatchQuery(string Key, List<object> Filter)
        {
            string projectquery = "";
            string matchquery = "";
            string mongolist = "";
            List<string> aggregatecommand = new List<string>();
            for (int i = 0; i < Filter.Count; i++)
            {
                if (mongolist != "")
                {

                    mongolist = mongolist + "," + Filter[i];
                }
                else
                { 
                        mongolist = Filter[i].ToString();
                }
            }
          //  projectquery = "{$match: {isinfilterlist: {$in: [" + "\"$" + Key + "\"," + "[" + mongolist + "]]}}}";
             projectquery = "{$addFields: {isinfilterlist: {$in: [" + "\"$" + Key + "\"," + "[" + mongolist + "]]}}}"; 
             matchquery= "{$match:{" +"isinfilterlist" + ":true}}";

            aggregatecommand.Add(projectquery);
            aggregatecommand.Add(matchquery);

            return aggregatecommand;
            
        }

    }
}
