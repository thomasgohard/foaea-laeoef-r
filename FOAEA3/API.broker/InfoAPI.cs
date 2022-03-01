using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class InfoAPI : BaseAPI
    {
        internal string GetMainDBConnectionString()
        {
            return GetStringAsync("api/v1/APIConfigurations/MainDBCconnectionString").Result;
        }

        internal string GetVersion()
        {
            return GetStringAsync("api/v1/APIConfigurations/Version").Result;
        }

        internal MessageDataList GetMessages()
        {
            return GetDataAsync<MessageDataList>($"api/v1/APIConfigurations/Messages").Result;
        }
    }
}
