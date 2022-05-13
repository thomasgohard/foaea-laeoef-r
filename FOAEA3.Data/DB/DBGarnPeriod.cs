using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Data.DB
{
    internal class DBGarnPeriod : DBbase, IGarnPeriodRepository
    {
        public DBGarnPeriod(IDBTools mainDB) : base(mainDB)
        {

        }
        
        public void UpdateGarnPeriod(string applEnfSrvCd,
                                       string applCtrlCd,
                                       decimal finTrmLumpSumAmt,
                                       decimal finTrmPerPymAmt,
                                       DateTime calcStartDate,
                                       ref decimal lumpDivertedTtl,
                                       ref decimal prdPymtDivertedTtl)
        {

            GetDivertedTotalsForVaryAccept(applEnfSrvCd, applCtrlCd, calcStartDate, ref prdPymtDivertedTtl, ref lumpDivertedTtl);

            InsertGarnPeriodVary(applEnfSrvCd, applCtrlCd);
            DeleteGarnPeriod(applEnfSrvCd, applCtrlCd);

            if (finTrmLumpSumAmt < lumpDivertedTtl)
            {
                prdPymtDivertedTtl += (lumpDivertedTtl - finTrmLumpSumAmt);
                lumpDivertedTtl = finTrmLumpSumAmt;
            }

            if (finTrmPerPymAmt == 0)
            {
                lumpDivertedTtl += prdPymtDivertedTtl;
                prdPymtDivertedTtl = 0;
            }
            else
            {
                int loopIndex = 1;
                decimal distPymtDivAmt = prdPymtDivertedTtl;

                while (distPymtDivAmt > 0)
                {
                    decimal perAmt;
                    if(distPymtDivAmt > finTrmPerPymAmt)
                    {
                        perAmt = finTrmPerPymAmt;
                        distPymtDivAmt -= perAmt;
                    }
                    else
                    {
                        perAmt = distPymtDivAmt;
                        distPymtDivAmt = 0;
                    }

                    InsertGarnPeriod(applEnfSrvCd, applCtrlCd, 0, loopIndex, perAmt);

                    loopIndex += 1;
                }

            }

        }

        private void InsertGarnPeriod(string applEnfSrvCd, string applCtrlCd, int summFAFRId, int periodcnt, decimal garnAmt)
        {
            var parameters = new Dictionary<string, object>
            {
                {"chrAppl_EnfSrv_Cd", applEnfSrvCd},
                {"chrAppl_CtrlCd", applCtrlCd},
                {"IntSummFAFR_Id",  summFAFRId},
                {"Periodcnt",  periodcnt},
                {"GarnAmt",  garnAmt}
            };

            MainDB.ExecProc("GarnPeriodInsert", parameters);
        }

        private void DeleteGarnPeriod(string applEnfSrvCd, string applCtrlCd)
        {
            var parameters = new Dictionary<string, object>
            {
                {"chrAppl_EnfSrv_Cd", applEnfSrvCd},
                {"chrAppl_CtrlCd", applCtrlCd}
            };

            MainDB.ExecProc("GarnPeriodDeleteForAppl", parameters);
        }

        private void InsertGarnPeriodVary(string applEnfSrvCd, string applCtrlCd)
        {
            var parameters = new Dictionary<string, object>
            {
                {"chrAppl_EnfSrv_Cd", applEnfSrvCd},
                {"chrAppl_CtrlCd", applCtrlCd}
            };

            MainDB.ExecProc("GarnPeriodVaryInsert", parameters);
        }

        private class UpdatedDiverts
        {
            public decimal PrdPymtDivertedTtl { get; set; }
            public decimal LumpDivertedTtl { get; set; }
        }

        private void GetDivertedTotalsForVaryAccept(string applEnfSrvCd, string applCtrlCd, DateTime calcStartDate, 
                                                    ref decimal prdPymtDivertedTtl, ref decimal lumpDivertedTtl)
        {
            var parameters = new Dictionary<string, object>
            {
                {"chrAppl_EnfSrv_Cd", applEnfSrvCd},
                {"chrAppl_CtrlCd", applCtrlCd},
                {"calcStartDate", calcStartDate}
            };

            var result = MainDB.GetDataFromStoredProc<UpdatedDiverts>("GetDivertedTotalsForVaryAccept", parameters, FillDataFromReader).FirstOrDefault();

            if (result is not null)
            {
                prdPymtDivertedTtl = result.PrdPymtDivertedTtl;
                lumpDivertedTtl = result.LumpDivertedTtl;
            }           

        }

        private void FillDataFromReader(IDBHelperReader rdr, UpdatedDiverts data)
        {
            data.PrdPymtDivertedTtl = (decimal) rdr["prdPymtDivertedTtl"] ;
            data.LumpDivertedTtl = (decimal) rdr["lumpDivertedTtl"];
        }
    }
}
