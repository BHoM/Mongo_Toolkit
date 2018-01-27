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
        public static string MongoAdd(string Key, List<object> Operands)
        {
            string aggregatecommand = "";
            string Operand_Array = "";
            int i = 0;

            foreach (object x in Operands)
            {

                if ( x is  double || x is int)
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
                    if (i == 0)
                    {

                        Operand_Array = "\"$"+x.ToString()+"\"";
                    }
                    else

                    {
                        string asdf = ",\"$" + x + "\"";
                        Operand_Array = Operand_Array + asdf ;
                    }
                }
                else
                {
                   return aggregatecommand = "Error, Operand must be a double, integer, or a mongo db property";
                }

                i++;

            }

            aggregatecommand = "{$addFields: {" + Key + ":" + "{$sum: [" + Operand_Array + "] }}}";
            return aggregatecommand;
        }

    }
}