using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    public class DBApplicationReason : DBbase, IApplicationReasonRepository
    {
        public MessageDataList Messages { get; set; }

        public DBApplicationReason(IDBToolsAsync mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public async Task<DataList<ApplicationReasonData>> GetApplicationReasonsAsync()
        {
            var data = await MainDB.GetAllDataAsync<ApplicationReasonData>("AppReas", FillApplicationReasonDataFromReader);

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
