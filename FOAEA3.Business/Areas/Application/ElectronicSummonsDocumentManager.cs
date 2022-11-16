using FOAEA3.Common.Helpers;
using FOAEA3.Common.Models;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO.Compression;

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

        public async Task<ElectronicSummonsDocumentZipData> CreateESD(ElectronicSummonsDocumentZipData newData)
        {
            var newESDzip = await DB.InterceptionTable.CreateESDasync(newData.PrcID, newData.ZipName, newData.DateReceived);

            using (ZipArchive archive = ZipFile.OpenRead(newESDzip.ZipName))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string pdfName = entry.FullName;

                    // TODO: Add PDF info the ZIPPDFs table

                }
            }

            // TODO: update ESDRequired table

            return newESDzip;
        }
    }
}
