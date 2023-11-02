using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBApplicationComments : DBbase, IApplicationCommentsRepository
    {
        public DBApplicationComments(IDBToolsAsync mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public MessageDataList Messages { get; set; }

        public async Task<DataList<ApplicationCommentsData>> GetApplicationComments()
        {

            var data = await MainDB.GetAllDataAsync<ApplicationCommentsData>("ApplicationComments", FillDataFromReader);

            return new DataList<ApplicationCommentsData>(data, MainDB.LastError);

        }

        private void FillDataFromReader(IDBHelperReader rdr, ApplicationCommentsData data)
        {
            data.ApplicationCommentsId = (int)rdr["ApplicationCommentsId"];
            data.ApplicationComments_Txt_E = rdr["ApplicationComments_Txt_E"] as string; // can be null 
            data.ApplicationComments_Txt_F = rdr["ApplicationComments_Txt_F"] as string; // can be null 
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string; // can be null
        }
    }
}
