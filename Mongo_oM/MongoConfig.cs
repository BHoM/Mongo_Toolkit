using BH.oM.Adapter;
using System;
using System.ComponentModel;

namespace BH.oM.Adapter.Mongo
{
    public class MongoConfig : ActionConfig
    {
        [Description("Replace the content while actioning Execute.")]
        public bool Replace { get; set; } = false;
        public string Tag { get; set; } = "";
    }
}
