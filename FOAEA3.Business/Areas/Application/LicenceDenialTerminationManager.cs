using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using FOAEA3.Data.Base;
using FOAEA3.Business.Security;
using FOAEA3.Model.Enums;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class LicenceDenialTerminationManager : ApplicationManager
    {
        public LicenceDenialApplicationData LicenceDenialTerminationApplication { get; }

        public LicenceDenialTerminationManager(LicenceDenialApplicationData licenceDenialTermination, IRepositories repositories, CustomConfig config) :
            base(licenceDenialTermination, repositories, config)
        {
            LicenceDenialTerminationApplication = licenceDenialTermination;
        }

        public LicenceDenialTerminationManager(IRepositories repositories, CustomConfig config) :
            this(new LicenceDenialApplicationData(), repositories, config)
        {

        }

        public override bool LoadApplication(string enfService, string controlCode)
        {
            // get data from Appl
            bool isSuccess = base.LoadApplication(enfService, controlCode);

            if (isSuccess)
            {
                // get additional data from LicSusp table 
                var licenceDenialDB = Repositories.LicenceDenialRepository;
                var data = licenceDenialDB.GetLicenceDenialData(enfService, appl_L03_CtrlCd: controlCode);

                if (data != null)
                    LicenceDenialTerminationApplication.Merge(data);
            }

            return isSuccess;
        }

        public bool CreateApplication(string controlCodeForL01)
        {
            if (!IsValidCategory("L03"))
                return false;

            bool success = base.CreateApplication();

            if (!success)
            {
                var failedSubmitterManager = new FailedSubmitAuditManager(Repositories, LicenceDenialTerminationApplication);
                failedSubmitterManager.AddToFailedSubmitAudit(FailedSubmitActivityAreaType.L03);
            }

            Repositories.LicenceDenialRepository.CreateLicenceDenialData(LicenceDenialTerminationApplication);

            return success;
        }

    }
}
