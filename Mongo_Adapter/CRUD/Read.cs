using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        protected override IEnumerable<BHoMObject> Read(Type type, IList ids)
        {
            return new List<BHoMObject>();
        }

        /***************************************************/
    }
}
