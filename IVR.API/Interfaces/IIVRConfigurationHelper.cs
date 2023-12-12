using FOAEA3.IVR.Model;
using System;
using System.Collections.Generic;

namespace FOAEA3.IVR.Interfaces
{
    public interface IIVRConfigurationHelper
    {
        string FoaeaConnection { get; }
        public RecipientsConfig Recipients { get; }
        //public string FoaeaIVRConnection { get; }
        public List<string> ProductionServers { get; }
    }
}
