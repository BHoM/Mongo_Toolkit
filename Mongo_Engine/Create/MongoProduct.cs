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
        public static string MongoProduct(List<object> Operands)
        {
            string mongoExpression = "";
            List<object> productArray = Operands;
            mongoExpression = "{$multiply: [" + MongoCleanVariable(productArray, mongoExpression) + "] }";
            return mongoExpression;
        }

    }
}