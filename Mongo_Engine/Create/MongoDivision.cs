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
        public static string MongoDivision(object Numerator, object Denominator)
        {
            string aggregatecommand = "";
            string DivideVariableArray = "";

            //process numerator
            if (Numerator is double || Numerator is int)
            {
                    DivideVariableArray = Numerator.ToString();
            }
            else if (Numerator is string)
            {
                if (Numerator.ToString().StartsWith("{"))  //if you are a mongo expresssion no need for quotes
                {
                    DivideVariableArray = Numerator.ToString();
                }
                else //if you are just a string, we need to add quotes and a $ to you
                {
                    DivideVariableArray = "\"$" + Numerator.ToString() + "\"";
                }
            }

            //process denominator
            if (Denominator is double || Denominator is int)
            {
                DivideVariableArray = DivideVariableArray+ ","+ Denominator.ToString();
            }
            else if (Denominator is string)
            {
                if (Denominator.ToString().StartsWith("{"))  //if you are a mongo expresssion no need for quotes
                {
                    DivideVariableArray = DivideVariableArray + "," + Denominator.ToString();
                }
                else //if you are just a string, we need to add quotes and a $ to you
                {
                    DivideVariableArray = DivideVariableArray+","+ "\"$" + Denominator.ToString() + "\"";
                }
            }

            aggregatecommand = "{$divide: [" + DivideVariableArray + "] }";

            return aggregatecommand;
        }


    }
}