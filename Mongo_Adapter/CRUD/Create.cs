using System.Collections.Generic;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        protected override bool Create<T>(IEnumerable<T> objects, bool replaceAll = false) 
        {
            return false;
        }

        /***************************************************/
    }
}
