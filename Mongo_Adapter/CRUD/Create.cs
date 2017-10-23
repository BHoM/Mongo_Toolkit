using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter
    {
        protected override bool Create<T>(IEnumerable<T> objects, bool replaceAll = false) 
        {
            return false;
        }
    }
}
