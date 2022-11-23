using FOAEA3.Common.Helpers;
using FOAEA3.Common.Models;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Application
{
    internal class ElectronicSummonsDocumentManager
    {
        private IRepositories DB { get; }
        private FoaeaUser _currentUser;

        public FoaeaUser CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;

                if (DB is not null)
                    DB.CurrentSubmitter = this.CurrentUser.Submitter.Subm_SubmCd;
            }
        }

        public ElectronicSummonsDocumentManager(IRepositories repositories)
        {
            DB = repositories;
        }

        public async Task SetCurrentUserAsync(ClaimsPrincipal user)
        {
            CurrentUser = await UserHelper.ExtractDataFromUser(user, DB);
        }

        public async Task<ElectronicSummonsDocumentZipData> GetESDasync(string fileName)
        {
            return await DB.InterceptionTable.GetESDasync(fileName);
        }

        public async Task<List<ElectronicSummonsDocumentData>> FindDocumentsForApplicationAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            if (appl_EnfSrv_Cd.Length > 2)
                appl_EnfSrv_Cd = appl_EnfSrv_Cd[0..2];
            return await DB.InterceptionTable.FindDocumentsForApplicationAsync(appl_EnfSrv_Cd, appl_CtrlCd);
        }

        public async Task<ElectronicSummonsDocumentZipData> CreateESD(ElectronicSummonsDocumentZipData newData)
        {
            return await DB.InterceptionTable.CreateESDasync(newData.PrcID, newData.ZipName, newData.DateReceived);
        }

        public async Task<ElectronicSummonsDocumentPdfData> CreateESDPDFasync(ElectronicSummonsDocumentPdfData pdfData)
        {
            var isAmendment = pdfData.PDFName.EndsWith("A.PDF");

            var newPdf = await DB.InterceptionTable.CreateESDPDFasync(pdfData);

            if (isAmendment)
            {
                var application = await DB.ApplicationTable.GetApplicationAsync(pdfData.EnfSrv, pdfData.Ctrl);
                if (application != null)
                {
                    if ((int)application.AppLiSt_Cd < 7)
                        await DB.InterceptionTable.InsertESDrequiredAsync(pdfData.EnfSrv, pdfData.Ctrl, ESDrequired.NoESDrequired);
                }
                else
                {
                    newPdf.Messages.AddWarning("Warning: Appl Record does not exist");
                }
            }

            return newPdf;

        }

    }
}
