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
            string mongoExpression = "";
            string tempVar = "";
            if (Items.Count==1)
            {
                mongoExpression = "{$addFields: {" + Key + ":" + BH.Engine.Mongo.Create.MongoCleanVariable(Items, tempVar) + "}}";
            }
            else
            {
                mongoExpression = "{$addFields: {" + Key + ":[" + BH.Engine.Mongo.Create.MongoCleanVariable(Items, tempVar) + "]}}";
            }
            return mongoExpression;
        }

    }
}