using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    public class DBDocumentType : DBbase, IDocumentTypeRepository
    {
        public MessageDataList Messages { get; set; }

        public DBDocumentType(IDBToolsAsync mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public async Task<DataList<DocumentTypeData>> GetDocumentTypesAsync()
        {
            var data = await MainDB.GetAllDataAsync<DocumentTypeData>("DocTyp", FillDocumentTypeDataFromReader);

            return new DataList<DocumentTypeData>(data, MainDB.LastError);
        }

        private void FillDocumentTypeDataFromReader(IDBHelperReader rdr, DocumentTypeData data)
        {
            data.DocTyp_Cd = rdr["DocTyp_Cd"] as string;
            data.DocTyp_Txt_E = rdr["DocTyp_Txt_E"] as string;
            data.DocTyp_Txt_F = rdr["DocTyp_Txt_F"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.AppCtgy_Cd = rdr["AppCtgy_Cd"] as string; // can be null  
        }
    }
}
