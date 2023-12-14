using FOAEA3.IVR.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.IVR.Interfaces.Repository
{
    public interface IIVRRepository
    {
        Task<CheckCreditorIdReturnData> CheckCreditorId(CheckCreditorIdGetData data);
        Task<CheckControlCodeReturnData> CheckControlCode(CheckControlCodeGetData data);
        Task<CheckDebtorIdReturnData> CheckDebtorId(CheckDebtorIdGetData data);
        Task<CheckDebtorLetterReturnData> CheckDebtorLetter(CheckDebtorLetterGetData data);
        Task<CheckSinReturnData> CheckSinCount(CheckSinGetData data);
        Task<GetAgencyReturnData> GetAgency(GetAgencyGetData data);
        Task<GetAgencyDebReturnData> GetAgencyDeb(GetAgencyDebGetData data);
        Task<GetApplControlCodeReturnData> GetApplControlCode(GetApplControlCodeGetData data);
        Task<GetApplEnforcementCodeReturnData> GetApplEnforcementCode(GetApplEnforcementCodeGetData data);
        Task<GetHoldbackConditionReturnData> GetHoldbackCondition(GetHoldbackConditionGetData data);
        Task<GetL01AgencyReturnData> GetL01Agency(GetL01AgencyGetData data);
        Task<List<GetPaymentsReturnData>> GetPayments(GetPaymentsGetData data);
        Task<GetSummonsReturnData> GetSummons(GetSummonsGetData data);
    }
}
