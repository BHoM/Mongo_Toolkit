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
        public static string AddField(string Key, List<object> Items)
        {
            string MongoExpression = "";
            string AddedVariableArray = "";
            int i = 0;

            foreach (object x in Items)
            {
                if (x is double || x is int)
                {
                    if (i == 0)
                    {

                        AddedVariableArray = x.ToString();
                    }
                    else

                    {
                        AddedVariableArray = AddedVariableArray + "," + x;
                    }
                }
                else if (x is string)
                {
                    if (!x.ToString().StartsWith("{"))  //if you are a mongo expresssion no need for quotes
                    {
                        if (i == 0)
                        {
                            AddedVariableArray = "\"" + x.ToString() + "\"";
                        }
                        else
                        {
                            string append = ",\"" + x + "\"";
                            AddedVariableArray = AddedVariableArray + append;
                        }
                    }
                    else //if you are just a string, we need to add quotes to you
                    {
                        if (i == 0)
                        {
                            AddedVariableArray =  x.ToString();
                        }
                        else
                        {
                            string append = "," + x;
                            AddedVariableArray = AddedVariableArray + append;
                        }
                    }
                }
                else
                {
                    return MongoExpression = "Error, Operand must be a double, integer, or a mongo db document property";
                }

                i++;

            }

            MongoExpression = "{$addFields: {" + Key + ":[" + AddedVariableArray + "] }}";


            return MongoExpression;
        }

    }
}