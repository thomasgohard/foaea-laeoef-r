using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System.Collections.Generic;
using FOAEA3.Model.Base;

namespace FOAEA3.Data.DB
{
    public class DBApplicationLifeState : DBbase, IApplicationLifeStateRepository
    {
        public DBApplicationLifeState(IDBTools mainDB) : base(mainDB)
        {

        }

        public DataList<ApplicationLifeStateData> GetApplicationLifeStates()
        {
            var data = MainDB.GetAllData<ApplicationLifeStateData>("AppLiSt", FillApplicationLifeStateDataFromReader);

            return new DataList<ApplicationLifeStateData>(data, MainDB.LastError);
        }

        private void FillApplicationLifeStateDataFromReader(IDBHelperReader rdr, ApplicationLifeStateData data)
        {
            data.AppLiSt_Cd = (ApplicationState)rdr["AppLiSt_Cd"];
            data.AppList_Txt_E = rdr["AppList_Txt_E"] as string; // can be null 
            data.AppList_Txt_F = rdr["AppList_Txt_F"] as string; // can be null 
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }
    }
}
