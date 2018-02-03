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
        public static string MongoPower(object Base, object Exponent)
        {
            string mongoExpression = "";
            List<object> powArray = new List<object> { Base, Exponent };
            mongoExpression = "{$pow: [" + MongoCleanVariable(powArray, mongoExpression) + "] }";
            return mongoExpression;
        }


    }
}