using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBApplicationCategory : DBbase, IApplicationCategoryRepository
    {
        public MessageDataList Messages { get; set; }

        public DBApplicationCategory(IDBToolsAsync mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public async Task<DataList<ApplicationCategoryData>> GetApplicationCategories()
        {
            var data = await MainDB.GetAllDataAsync<ApplicationCategoryData>("AppCtgy", FillApplicationCategoryDataFromReader);

            return new DataList<ApplicationCategoryData>(data, MainDB.LastError);
        }

        private void FillApplicationCategoryDataFromReader(IDBHelperReader rdr, ApplicationCategoryData data)
        {
            data.AppCtgy_Cd = rdr["AppCtgy_Cd"] as string;
            data.AppCtgy_Txt_E = rdr["AppCtgy_Txt_E"] as string;
            data.AppCtgy_Txt_F = rdr["AppCtgy_Txt_F"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }
    }
}
