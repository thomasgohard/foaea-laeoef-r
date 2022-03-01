using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System.Collections.Generic;
using FOAEA3.Model.Base;

namespace FOAEA3.Data.DB
{
    public class DBGender : DBbase, IGenderRepository
    {
        public MessageDataList Messages { get; set; }

        public DBGender(IDBTools mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public DataList<GenderData> GetGenders()
        {
            var data = MainDB.GetAllData<GenderData>("Gendr", FillGenderDataFromReader);

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
