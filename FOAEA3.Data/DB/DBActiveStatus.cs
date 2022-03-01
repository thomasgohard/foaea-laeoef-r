using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System.Collections.Generic;
using System;
using FOAEA3.Model.Base;

namespace FOAEA3.Data.DB
{
    public class DBActiveStatus : DBbase, IActiveStatusRepository
    {

        public MessageDataList Messages { get; set;  }

        public DBActiveStatus(IDBTools mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public DataList<ActiveStatusData> GetActiveStatus()
        {
            var data = MainDB.GetAllData<ActiveStatusData>("ActvSt", FillActiveStatusDataFromReader);

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
