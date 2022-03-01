using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using FOAEA3.Data.Base;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class LicenceDenialTerminationManager : ApplicationManager
    {
        public LicenceDenialData LicenceDenialTerminationApplication { get; }

        public LicenceDenialTerminationManager(LicenceDenialData licenceDenialTermination, IRepositories repositories, CustomConfig config) :
            base(licenceDenialTermination, repositories, config)
        {
            LicenceDenialTerminationApplication = licenceDenialTermination;
        }

        public LicenceDenialTerminationManager(IRepositories repositories, CustomConfig config) :
            this(new LicenceDenialData(), repositories, config)
        {

        }

        public override bool LoadApplication(string enfService, string controlCode)
        {
            // get data from Appl
            bool isSuccess = base.LoadApplication(enfService, controlCode);

            if (isSuccess)
            {
                // get additional data from LicSusp table 
                LicenceDenialData data = Repositories.LicenceDenialRepository.GetLicenceDenialData(enfService, controlCode);

                if (data != null)
                    LicenceDenialTerminationApplication.Merge(data);
            }

            return isSuccess;
        }
    }
}
