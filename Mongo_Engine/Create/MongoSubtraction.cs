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
        public static string MongoSubtraction(object A, object B)
        {
            string mongoExpression = "";
            List<object> SubtractVariableArray = new List<object> { A, B };
            mongoExpression = "{$subtract: [" + MongoCleanVariable(SubtractVariableArray,mongoExpression) + "] }";
            return mongoExpression;
        }
    }
}