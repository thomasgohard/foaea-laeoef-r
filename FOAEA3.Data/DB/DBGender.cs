using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    public class DBGender : DBbase, IGenderRepository
    {
        public MessageDataList Messages { get; set; }

        public DBGender(IDBToolsAsync mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public async Task<DataList<GenderData>> GetGendersAsync()
        {
            var data = await MainDB.GetAllDataAsync<GenderData>("Gendr", FillGenderDataFromReader);

            return new DataList<GenderData>(data, MainDB.LastError);
        }

        private void FillGenderDataFromReader(IDBHelperReader rdr, GenderData data)
        {
            data.Gender_Cd = rdr["Gendr_Cd"] as string;
            data.Gendr_Txt_E = rdr["Gendr_Txt_E"] as string; // can be null 
            data.Gendr_Txt_F = rdr["Gendr_Txt_F"] as string; // can be null 
        }
    }

}
