using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Data.DB
{
    internal class DBSummDF : DBbase, ISummDFRepository
    {
        internal DBSummDF(IDBTools mainDB) : base(mainDB)
        {

        }

        public DataList<SummDF_Data> GetSummDFList(int summFAFR_Id)
        {
            throw new NotImplementedException();
        }

        private void FillDataFromReader(IDBHelperReader rdr, SummDF_Data data)
        {
            data.SummDF_Id = (int)rdr["SummDF_Id"];
            data.SummFAFR_Id = (int)rdr["SummFAFR_Id"];
            data.Batch_Id = rdr["Batch_Id"] as string;
            data.IntFinH_Dte = (DateTime)rdr["IntFinH_Dte"];
            data.SummDF_Divert_Dte = (DateTime)rdr["SummDF_Divert_Dte"];
            data.SummDF_CurrPerCnt = rdr["SummDF_CurrPerCnt"] as int?; // can be null 
            data.SummDF_ProRatePerc = rdr["SummDF_ProRatePerc"] as decimal?; // can be null 
            data.SummDF_MultiSumm_Ind = rdr["SummDF_MultiSumm_Ind"] as byte?; // can be null 
            data.SummDF_LmpSumDueAmt_Money = rdr["SummDF_LmpSumDueAmt_Money"] as decimal?; // can be null 
            data.SummDF_PerPymDueAmt_Money = rdr["SummDF_PerPymDueAmt_Money"] as decimal?; // can be null 
            data.SummDF_DivertedDbtrAmt_Money = rdr["SummDF_DivertedDbtrAmt_Money"] as decimal?; // can be null 
            data.SummDF_DivOrigDbtrAmt_Money = rdr["SummDF_DivOrigDbtrAmt_Money"] as decimal?; // can be null 
            data.SummDF_DivertedAmt_Money = rdr["SummDF_DivertedAmt_Money"] as decimal?; // can be null 
            data.SummDF_DivOrigAmt_Money = rdr["SummDF_DivOrigAmt_Money"] as decimal?; // can be null 
            data.SummDF_HldbAmt_Money = rdr["SummDF_HldbAmt_Money"] as decimal?; // can be null 
            data.SummDF_FeeAmt_Money = rdr["SummDF_FeeAmt_Money"] as decimal?; // can be null 
            data.SummDF_LmpSumAmt_Money = rdr["SummDF_LmpSumAmt_Money"] as decimal?; // can be null 
            data.SummDF_PerPymAmt_Money = rdr["SummDF_PerPymAmt_Money"] as decimal?; // can be null 
            data.SummDF_MiscAmt_Money = rdr["SummDF_MiscAmt_Money"] as decimal?; // can be null 
        }

    }

}
