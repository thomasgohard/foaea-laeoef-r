using DBHelper;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using System.Collections.Generic;

namespace FileBroker.Data.DB
{
    public class DBProcessParameter : IProcessParameterRepository
    {
        private IDBTools MainDB { get; }

        public DBProcessParameter(IDBTools mainDB)
        {
            MainDB = mainDB;
        }

        public string GetValueForParameter(int processId, string parameter)
        {
            var parameters = new Dictionary<string, object>
            {
                {"rsntProcess_Cd", processId},
                {"rvchProps_Cd", parameter}
            };

            string outputParamFieldName = "dvchOutput";

            return MainDB.GetDataFromStoredProcViaReturnParameter<string>("MessageBrokerConfigGetProcessParamterValue", parameters, outputParamFieldName);
        }

        public ProcessCodeData GetProcessCodes(int processId)
        {

            var parameters = new Dictionary<string, object>
            {
                { "rsntProcess_Cd", processId }
            };

            var outputParameters = new Dictionary<string, string>
            {
                { "EnfSrv_Cd",       "S4" },
                { "ProcessCategory", "S12" },
                { "ActvSt_Cd",       "S4" },
                { "SubmRecptCd",     "S8" },
                { "AppLiSt_Cd",      "I" },
                { "EnfSrv_Loc_Cd",   "S4" }
            };

            var values = MainDB.GetDataFromStoredProcViaReturnParameters("MessageBrokerConfigGetProcessCodes",
                                                                         parameters, outputParameters);

            var processCodes = new ProcessCodeData
            {
                EnfSrv_Cd = values["EnfSrv_Cd"] as string,
                ProcessCategory = values["ProcessCategory"] as string,
                ActvSt_Cd = values["ActvSt_Cd"] as string,
                SubmRecptCd = values["SubmRecptCd"] as string,
                AppLiSt_Cd = (int)values["AppLiSt_Cd"],
                EnfSrv_Loc_Cd = values["EnfSrv_Loc_Cd"] as string,
            };

            return processCodes;
        }

    }
}
