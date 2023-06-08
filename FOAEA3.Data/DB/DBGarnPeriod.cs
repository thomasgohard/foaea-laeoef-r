using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBGarnPeriod : DBbase, IGarnPeriodRepository
    {
        public DBGarnPeriod(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<(decimal, decimal)> UpdateGarnPeriod(string applEnfSrvCd,
                                                                    string applCtrlCd,
                                                                    decimal finTrmLumpSumAmt,
                                                                    decimal finTrmPerPymAmt,
                                                                    DateTime calcStartDate,
                                                                    decimal lumpDivertedTtl,
                                                                    decimal prdPymtDivertedTtl)
        {

            (prdPymtDivertedTtl, lumpDivertedTtl) = await GetDivertedTotalsForVaryAccept(applEnfSrvCd, applCtrlCd, calcStartDate, prdPymtDivertedTtl, lumpDivertedTtl);

            await InsertGarnPeriodVary(applEnfSrvCd, applCtrlCd);
            await DeleteGarnPeriod(applEnfSrvCd, applCtrlCd);

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
                    if (distPymtDivAmt > finTrmPerPymAmt)
                    {
                        perAmt = finTrmPerPymAmt;
                        distPymtDivAmt -= perAmt;
                    }
                    else
                    {
                        perAmt = distPymtDivAmt;
                        distPymtDivAmt = 0;
                    }

                    await InsertGarnPeriod(applEnfSrvCd, applCtrlCd, 0, loopIndex, perAmt);

                    loopIndex += 1;
                }

            }

            return (lumpDivertedTtl, prdPymtDivertedTtl);

        }

        private async Task InsertGarnPeriod(string applEnfSrvCd, string applCtrlCd, int summFAFRId, int periodcnt, decimal garnAmt)
        {
            var parameters = new Dictionary<string, object>
            {
                {"chrAppl_EnfSrv_Cd", applEnfSrvCd},
                {"chrAppl_CtrlCd", applCtrlCd},
                {"IntSummFAFR_Id",  summFAFRId},
                {"Periodcnt",  periodcnt},
                {"GarnAmt",  garnAmt}
            };

            await MainDB.ExecProcAsync("GarnPeriodInsert", parameters);
        }

        private async Task DeleteGarnPeriod(string applEnfSrvCd, string applCtrlCd)
        {
            var parameters = new Dictionary<string, object>
            {
                {"chrAppl_EnfSrv_Cd", applEnfSrvCd},
                {"chrAppl_CtrlCd", applCtrlCd}
            };

            await MainDB.ExecProcAsync("GarnPeriodDeleteForAppl", parameters);
        }

        private async Task InsertGarnPeriodVary(string applEnfSrvCd, string applCtrlCd)
        {
            var parameters = new Dictionary<string, object>
            {
                {"chrAppl_EnfSrv_Cd", applEnfSrvCd},
                {"chrAppl_CtrlCd", applCtrlCd}
            };

            await MainDB.ExecProcAsync("GarnPeriodVaryInsert", parameters);
        }

        private class UpdatedDiverts
        {
            public decimal PrdPymtDivertedTtl { get; set; }
            public decimal LumpDivertedTtl { get; set; }
        }

        private async Task<(decimal, decimal)> GetDivertedTotalsForVaryAccept(string applEnfSrvCd, string applCtrlCd, DateTime calcStartDate,
                                                          decimal prdPymtDivertedTtl, decimal lumpDivertedTtl)
        {
            var parameters = new Dictionary<string, object>
            {
                {"chrAppl_EnfSrv_Cd", applEnfSrvCd},
                {"chrAppl_CtrlCd", applCtrlCd},
                {"calcStartDate", calcStartDate}
            };

            var result = (await MainDB.GetDataFromStoredProcAsync<UpdatedDiverts>("GetDivertedTotalsForVaryAccept", parameters, FillDataFromReader)).FirstOrDefault();

            if (result is not null)
            {
                prdPymtDivertedTtl = result.PrdPymtDivertedTtl;
                lumpDivertedTtl = result.LumpDivertedTtl;
            }

            return (prdPymtDivertedTtl, lumpDivertedTtl);

        }

        private void FillDataFromReader(IDBHelperReader rdr, UpdatedDiverts data)
        {
            data.PrdPymtDivertedTtl = (decimal)rdr["prdPymtDivertedTtl"];
            data.LumpDivertedTtl = (decimal)rdr["lumpDivertedTtl"];
        }
    }
}
