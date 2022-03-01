using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBInfoBank : DBbase, IInfoBankRepository
    {
        public DBInfoBank(IDBTools mainDB) : base(mainDB)
        {

        }

        public List<InfoBankData> GetInfoBanks()
        {
            return MainDB.GetAllData<InfoBankData>("InfoBank", FillDataFromReader);
        }

        private void FillDataFromReader(IDBHelperReader rdr, InfoBankData data)
        {
            data.InfoBank_Cd = rdr["InfoBank_Cd"] as string;
            data.InfoBank_Txt_E = rdr["InfoBank_Txt_E"] as string;
            data.InfoBank_Txt_F = rdr["InfoBank_Txt_F"] as string;
            data.Prv_Cd = rdr["Prv_Cd"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }
    }
}
