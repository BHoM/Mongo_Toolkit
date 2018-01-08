using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.Mongo
{
    public class CreateMatchQuery
    {
        public List<string> CreateQuery(string Key, List<object> Filter, List<object> push)
        {
            string projectquery = "";
            string matchquery = "";
            string mongolist = "";
            List<string> aggregatecommand = new List<string>();
            for (int i = 0; i < Filter.Count; i++)
            {
                if (mongolist != "")
                {

                    mongolist = mongolist + "," + Filter[i];
                }
                else
                { 
                        mongolist = Filter[i].ToString();
                }
            }
            string propogation = existingfields(push);
            projectquery = "{$project: {isinfilterlist: {$in: [" + "\"$" + Key + "\"," + "[" + mongolist + "]]}" + propogation + "}}"; 
            matchquery= "{$match:{" +"isinfilterlist" + ":true}}";
            aggregatecommand.Add(matchquery);
            aggregatecommand.Add(projectquery);
           
            return aggregatecommand;
            
        }

        public string existingfields(List<object> propogate)
        {
            string propogatenew = "";
            //need code to add existing feilds to propogate
            for (int i = 0; i < propogate.Count ; i++)
            {
                propogatenew = propogatenew + "," + propogate[i] + ":1";

            }
                return propogatenew;
        }
    }
}
