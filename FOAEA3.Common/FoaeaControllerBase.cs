using FOAEA3.Common.Helpers;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.Common
{
    public class FoaeaControllerBase : ControllerBase
    {
        protected readonly IFoaeaConfigurationHelper config;

        public FoaeaControllerBase()
        {
            config = new FoaeaConfigurationHelper();
        }
    }
}
