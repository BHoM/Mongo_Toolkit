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
        public static string MongoAnd(List<object> Operands)
        {
            string mongoExpression = "";
            List<object> productArray = Operands;
            mongoExpression = "{$and: [" + MongoCleanVariable(productArray, mongoExpression) + "] }";
            return mongoExpression;
        }

    }
}