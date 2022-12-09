using DBHelper;
using FileBroker.Common.Helpers;
using System.Text;

namespace FileBroker.Business
{
    public partial class OutgoingFinancialEISOmanager
    {
        private APIBrokerList APIs { get; }
        private RepositoryList DB { get; }

        private FoaeaSystemAccess FoaeaAccess { get; }

        public OutgoingFinancialEISOmanager(APIBrokerList apis, RepositoryList repositories, IFileBrokerConfigurationHelper config)
        {
            APIs = apis;
            DB = repositories;

            FoaeaAccess = new FoaeaSystemAccess(apis, config.FoaeaLogin);
        }

        private static string FormatCycleEISO(int cycle)
        {
            if (cycle > 9999)
                cycle = 1;

            return cycle.ToString().PadLeft(4, '0');
        }
    }
}
