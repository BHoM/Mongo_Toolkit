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
        public static string MongoMultiplication(List<object> Operands)
        {
            string aggregatecommand = "";
            string Operand_Array = "";
            int i = 0;

            foreach (object x in Operands)
            {

                if (x is double || x is int)
                {
                    if (i == 0)
                    {

                        Operand_Array = x.ToString();
                    }
                    else

                    {
                        Operand_Array = Operand_Array + "," + x;
                    }
                }
                else if (x is string)
                {
                    if (!x.ToString().StartsWith("{"))  //if you are a mongo expresssion no need for quotes
                    {
                        if (i == 0)
                        {
                            Operand_Array = "\"$" + x.ToString() + "\"";
                        }
                        else

                        {
                            string asdf = ",\"$" + x + "\"";
                            Operand_Array = Operand_Array + asdf;
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            Operand_Array = x.ToString();
                        }
                        else

                        {
                            Operand_Array = Operand_Array + "," + x;
                        }
                    }
                }
                else
                {
                    return aggregatecommand = "Error, Operand must be a double, integer, or a mongo db property";
                }

                i++;

            }

            aggregatecommand = "{$multiply: [" + Operand_Array + "] }";
            return aggregatecommand;
        }

    }
}