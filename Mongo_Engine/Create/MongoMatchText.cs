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
        public static List<string> MongoMatchText(string Key, List<string> Filter)
        {
            string projectquery = "";
            string matchquery = "";
            string mongolist ="";
            string tempVar = "";
            List<string> aggregatecommand = new List<string>();

            int i = 0;
            foreach (string item in Filter)
            {
                if (i==0)
                     mongolist = "\"" + item+ "\"";
                else
                    mongolist = mongolist + ", \"" + item + "\"";
                i++;
            }

            projectquery = "{$addFields: {isinfilterlist_" + Key + " : {$in: [" + "\"$" + Key + "\" , " + "[" + mongolist + "]] } } }";
            matchquery = "{$match: { " + "isinfilterlist_" + Key + " : true} }";

            aggregatecommand.Add(projectquery);
            aggregatecommand.Add(matchquery);

            return aggregatecommand;

        }

    }
}