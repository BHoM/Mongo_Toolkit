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
            string aggregatecommand = "";
            string PowVariableArray = "";

            //process numerator
            if (Base is double || Base is int)
            {
                PowVariableArray = Base.ToString();
            }
            else if (Base is string)
            {
                if (Base.ToString().StartsWith("{"))  //if you are a mongo expresssion no need for quotes
                {
                    PowVariableArray = Base.ToString();
                }
                else //if you are just a string, we need to add quotes and a $ to you
                {
                    PowVariableArray = "\"$" + Base.ToString() + "\"";
                }
            }

            //process denominator
            if (Exponent is double || Exponent is int)
            {
                PowVariableArray = PowVariableArray + "," + Exponent.ToString();
            }
            else if (Exponent is string)
            {
                if (Exponent.ToString().StartsWith("{"))  //if you are a mongo expresssion no need for quotes
                {
                    PowVariableArray = PowVariableArray + "," + Exponent.ToString();
                }
                else //if you are just a string, we need to add quotes and a $ to you
                {
                    PowVariableArray = PowVariableArray + "," + "\"$" + Exponent.ToString() + "\"";
                }
            }

            aggregatecommand = "{$pow: [" + PowVariableArray + "] }";

            return aggregatecommand;
        }


    }
}