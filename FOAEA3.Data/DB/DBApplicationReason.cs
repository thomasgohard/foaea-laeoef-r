using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;

namespace FOAEA3.Data.DB
{
    public class DBApplicationReason : DBbase, IApplicationReasonRepository
    {
        public MessageDataList Messages { get; set; }

        public DBApplicationReason(IDBTools mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public DataList<ApplicationReasonData> GetApplicationReasons()
        {
            var data = MainDB.GetAllData<ApplicationReasonData>("AppReas", FillApplicationReasonDataFromReader);

            return new DataList<ApplicationReasonData>(data, MainDB.LastError);
        }

        private void FillApplicationReasonDataFromReader(IDBHelperReader rdr, ApplicationReasonData data)
        {
            data.AppReas_Cd = rdr["AppReas_Cd"] as string;
            data.AppReas_Txt_E = rdr["AppReas_Txt_E"] as string;
            data.AppReas_Txt_F = rdr["AppReas_Txt_F"] as string;
            data.AppCtgy_Cd = rdr["AppCtgy_Cd"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string; 
        }
    }
}
