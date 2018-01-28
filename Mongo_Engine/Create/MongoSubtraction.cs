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
            string aggregatecommand = "";
            string SubtractVariableArray = "";

            //process numerator
            if (A is double || A is int)
            {
                SubtractVariableArray = A.ToString();
            }
            else if (A is string)
            {
                if (A.ToString().StartsWith("{"))  //if you are a mongo expresssion no need for quotes
                {
                    SubtractVariableArray = A.ToString();
                }
                else //if you are just a string, we need to add quotes and a $ to you
                {
                    SubtractVariableArray = "\"$" + A.ToString() + "\"";
                }
            }

            //process denominator
            if (B is double || B is int)
            {
                SubtractVariableArray = SubtractVariableArray + "," + B.ToString();
            }
            else if (B is string)
            {
                if (B.ToString().StartsWith("{"))  //if you are a mongo expresssion no need for quotes
                {
                    SubtractVariableArray = SubtractVariableArray + "," + B.ToString();
                }
                else //if you are just a string, we need to add quotes and a $ to you
                {
                    SubtractVariableArray = SubtractVariableArray + "," + "\"$" + B.ToString() + "\"";
                }
            }

            aggregatecommand = "{$subtract: [" + SubtractVariableArray + "] }";

            return aggregatecommand;
        }


    }
}