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
        public static string MongoMax(string MongoArrayName)
        {
            //example mongo query:
            //{$addFields:      {$sort : { age : -1} }

            string outputquery = "";
            string mongoexpressionA = "{$max : \"$";
            string mongoexpressionB = "\"}";
            string maxexpression = mongoexpressionA + MongoArrayName + mongoexpressionB;
            List<object> maxexpressionlist = new List<object>();
            maxexpressionlist.Add(maxexpression);
            string key = "max_" + MongoArrayName;

            outputquery = MongoAddField(key , maxexpressionlist);

            return outputquery;
        }
    }
}