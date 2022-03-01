using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using FOAEA3.Model.Base;

namespace FOAEA3.Data.DB
{
    public class DBApplicationComments: DBbase, IApplicationCommentsRepository
    {
        public DBApplicationComments(IDBTools mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public MessageDataList Messages { get; set; }

        public DataList<ApplicationCommentsData> GetApplicationComments()
        {

            var data = MainDB.GetAllData<ApplicationCommentsData>("ApplicationComments", FillDataFromReader);

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
