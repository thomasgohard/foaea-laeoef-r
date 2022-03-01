using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class APIconfiguration
    {
        public string APIroot { get; }

        public APIconfiguration(string apiRoot)
        {
            APIroot = apiRoot;
        }
    }
}
