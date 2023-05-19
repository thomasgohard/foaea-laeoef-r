using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBMedium : DBbase, IMediumRepository
    {
        public MessageDataList Messages { get; set; }

        public DBMedium(IDBToolsAsync mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public async Task<DataList<MediumData>> GetMediumsAsync()
        {
            var data = await MainDB.GetAllDataAsync<MediumData>("Medium", FillMediumDataFromReader);

            return new DataList<MediumData>(data, MainDB.LastError);
        }

        private void FillMediumDataFromReader(IDBHelperReader rdr, MediumData data)
        {
            data.Medium_Cd = rdr["Medium_Cd"] as string;
            data.Medium_Txt_E = rdr["Medium_Txt_E"] as string;
            data.Medium_Txt_F = rdr["Medium_Txt_F"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }
    }
}
