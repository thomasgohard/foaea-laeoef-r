using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBActiveStatus : DBbase, IActiveStatusRepository
    {

        public MessageDataList Messages { get; set; }

        public DBActiveStatus(IDBToolsAsync mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public async Task<DataList<ActiveStatusData>> GetActiveStatus()
        {
            var data = await MainDB.GetAllDataAsync<ActiveStatusData>("ActvSt", FillActiveStatusDataFromReader);

            return new DataList<ActiveStatusData>(data, MainDB.LastError);
        }

        private void FillActiveStatusDataFromReader(IDBHelperReader rdr, ActiveStatusData data)
        {
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.ActvSt_Txt_E = rdr["ActvSt_Txt_E"] as string;
            data.ActvSt_Txt_F = rdr["ActvSt_Txt_F"] as string;
        }
    }



}
