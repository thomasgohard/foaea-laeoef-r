using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    public class DBLanguage : DBbase, ILanguageRepository
    {
        public MessageDataList Messages { get; set; }

        public DBLanguage(IDBToolsAsync mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public async Task<DataList<LanguageData>> GetLanguagesAsync()
        {
            var data = await MainDB.GetAllDataAsync<LanguageData>("Lng", FillLanguageDataFromReader);

            return new DataList<LanguageData>(data, MainDB.LastError);
        }

        private void FillLanguageDataFromReader(IDBHelperReader rdr, LanguageData data)
        {
            data.Lng_Cd = rdr["Lng_Cd"] as string;
            data.Lng_Txt_E = rdr["Lng_Txt_E"] as string;
            data.Lng_Txt_F = rdr["Lng_Txt_F"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }
    }
}
