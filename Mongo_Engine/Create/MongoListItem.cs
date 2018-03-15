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
        public static string MongoListItem(string MongoArrayName, object Index)
        {
            //example mongo query:
            //{$addFields: {stress:{$arrayElemAt:["$SectionArea.Area",0]}}}
            string outputquery = "";
            string mongoexpressionA = "{$arrayElemAt: [\"$";
            string mongoexpressionB = "] }";

            outputquery = mongoexpressionA + MongoArrayName + "\"," +Index+ mongoexpressionB;

            return outputquery;
        }
    }
}
   