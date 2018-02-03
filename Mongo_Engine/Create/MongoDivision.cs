﻿using System;
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
        public static string MongoDivision(object Numerator, object Denominator)
        {
            string mongoExpression = "";
            List<object> DivideVariableArray = new List<object>{ Numerator, Denominator };
            mongoExpression = "{$divide: [" + MongoCleanVariable(DivideVariableArray,mongoExpression )+ "] }";
            return mongoExpression;
        }
    }
}