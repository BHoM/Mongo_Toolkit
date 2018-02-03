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
        public static string MongoAbsVal(List<object> Item)
        {
            string mongoExpression = "";
            string tempVar = "";
            mongoExpression = mongoExpression+","+ "{$abs:" + BH.Engine.Mongo.Create.MongoCleanVariable(Item,tempVar) + " }";
            return mongoExpression.TrimStart(',');
        }
    }
}