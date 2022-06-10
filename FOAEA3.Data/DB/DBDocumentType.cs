using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;

namespace FOAEA3.Data.DB
{
    public class DBDocumentType : DBbase, IDocumentTypeRepository
    {
        public MessageDataList Messages { get; set; }

        public DBDocumentType(IDBTools mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public DataList<DocumentTypeData> GetDocumentTypes()
        {
            var data = MainDB.GetAllData<DocumentTypeData>("DocTyp", FillDocumentTypeDataFromReader);

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
