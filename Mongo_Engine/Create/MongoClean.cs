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
        //Need to clean variable from mongo operations.  PEMDAS functions can take in ints, doubles, and strings (mongo document properties or mongo expression strings)
        public static string MongoCleanVariable(List<object> cleanMe, string outputObject)
        {
            foreach (object item in cleanMe)
            {
                if (item is double || item is int) //if you give me a number, then we'll treat it as a number
                    outputObject = outputObject + "," + item.ToString();
                else if (item is string && item.ToString().StartsWith("{")) //a mongo expression is a JSON object and will start with a {
                    outputObject = outputObject + "," + item.ToString();
                else if (item is string && !item.ToString().StartsWith("{"))
                    outputObject = outputObject + "," + "\"$" + item.ToString() + "\""; //mongo document properties used as vairables must be preceded with a dollar sign
                else if (item is List<object>)
                    outputObject = MongoCleanVariable((List<object>)item, outputObject); //recursive definition to handle list of lists
            }
            return outputObject.TrimStart(',');
        }
    }
}