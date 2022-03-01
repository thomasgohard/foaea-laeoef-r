using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOAEA3.Model.Enums
{
    public enum BalanceSnapshotChangeType
    {
        VARIATION_ACCEPTED = 1,
        ONLINE_NEW_DF_NIGHTLY_PROCESS = 2,
        EI_RECEIVED = 3,
        CPP_RECEIVED = 4,
        APP_SUSPENDED = 5,
        PAYMENT_HISTORY_CORRECTIONS = 6
    }
}
