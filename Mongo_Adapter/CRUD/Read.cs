using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter
    {
        protected override IEnumerable<BHoMObject> Read(Type type, IList ids)
        {
            return new List<BHoMObject>();
        }
    }
}
