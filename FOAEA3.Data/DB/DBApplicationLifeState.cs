using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBApplicationLifeState : DBbase, IApplicationLifeStateRepository
    {
        public DBApplicationLifeState(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<DataList<ApplicationLifeStateData>> GetApplicationLifeStates()
        {
            var data = await MainDB.GetAllDataAsync<ApplicationLifeStateData>("AppLiSt", FillApplicationLifeStateDataFromReader);

            var cleanData = data.Where(m => m.AppLiSt_Cd != ApplicationState.UNDEFINED).ToList();

            return new DataList<ApplicationLifeStateData>(cleanData, MainDB.LastError);
        }

        private void FillApplicationLifeStateDataFromReader(IDBHelperReader rdr, ApplicationLifeStateData data)
        {
            short applicationStateValue = (short)rdr["AppLiSt_Cd"];
            if (Enum.IsDefined(typeof(ApplicationState), applicationStateValue))
                data.AppLiSt_Cd = (ApplicationState)applicationStateValue;
            else
                data.AppLiSt_Cd = ApplicationState.UNDEFINED;

            data.AppList_Txt_E = rdr["AppList_Txt_E"] as string; // can be null 
            data.AppList_Txt_F = rdr["AppList_Txt_F"] as string; // can be null 
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }
    }
}
