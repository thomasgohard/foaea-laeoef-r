using FOAEA3.Model.Interfaces.Repository;
using FOAEA3.Model.IVR;

namespace IVR.API.Data
{
    public class FakeDataIVR : IIVRRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task<CheckControlCodeReturnData> CheckControlCode(CheckControlCodeGetData data)
        {
            return Task.FromResult<CheckControlCodeReturnData>(
                new CheckControlCodeReturnData
                {
                    ControlCodeCount = 1
                });
        }

        public Task<CheckCreditorIdReturnData> CheckCreditorId(CheckCreditorIdGetData data)
        {
            return Task.FromResult<CheckCreditorIdReturnData>(
                new CheckCreditorIdReturnData
                {
                    CreditorCount = 1
                });
        }

        public Task<CheckDebtorIdReturnData> CheckDebtorId(CheckDebtorIdGetData data)
        {
            return Task.FromResult<CheckDebtorIdReturnData>(
                new CheckDebtorIdReturnData
                {
                    DebtorIdCount = 1
                });
        }

        public Task<CheckDebtorLetterReturnData> CheckDebtorLetter(CheckDebtorLetterGetData data)
        {
            return Task.FromResult<CheckDebtorLetterReturnData>(
                new CheckDebtorLetterReturnData
                {
                    LetterCount = 1
                });
        }

        public Task<GetAgencyReturnData> GetAgency(GetAgencyGetData data)
        {
            return Task.FromResult<GetAgencyReturnData>(
                new GetAgencyReturnData
                {
                    EnforcementCode = "AB01",
                    EnfOffAbbrCd = "A",
                    AgencyType = "M",
                    EnfOffOrgCd = "string",
                    EnfOffName = "string",
                    EnfOffAddrLn = "string",
                    EnfOffAddrLn1 = null,
                    EnfOffAddrCityNme = "string",
                    ProvCode = "AB",
                    EnfSrvAddrPCd = "A1B2C3",
                    TelNumber = "1234561234"
                });
        }

        public Task<GetAgencyDebReturnData> GetAgencyDeb(GetAgencyDebGetData data)
        {
            return Task.FromResult<GetAgencyDebReturnData>(
                new GetAgencyDebReturnData
                {
                    EnforcementCode = "AB01"
                });
        }

        public Task<GetApplControlCodeReturnData> GetApplControlCode(GetApplControlCodeGetData data)
        {
            return Task.FromResult<GetApplControlCodeReturnData>(
                new GetApplControlCodeReturnData
                {
                    ControlCode = "123456"
                });
        }

        public Task<GetApplEnforcementCodeReturnData> GetApplEnforcementCode(GetApplEnforcementCodeGetData data)
        {
            return Task.FromResult<GetApplEnforcementCodeReturnData>(
                new GetApplEnforcementCodeReturnData
                {
                    EnforcementCode = "AB01"
                });
        }

        public Task<GetHoldbackConditionReturnData> GetHoldbackCondition(GetHoldbackConditionGetData data)
        {
            return Task.FromResult<GetHoldbackConditionReturnData>(
                new GetHoldbackConditionReturnData
                {
                    EnfSrcCd = "RC",
                    HldbCtgCd = "0",
                    HldbMxmPerChq = "0",
                    HldbSrcHldValue = null
                });
        }

        public Task<GetL01AgencyReturnData> GetL01Agency(GetL01AgencyGetData data)
        {
            return Task.FromResult<GetL01AgencyReturnData>(
                new GetL01AgencyReturnData
                {
                    EnforcementCode = "AB01",
                    ActvStCd = "A",
                    ApplLglDte = "2010/10/10",
                    EnfOffAbbrCd = "A"
                });
        }

        public Task<List<GetPaymentsReturnData>> GetPayments(GetPaymentsGetData data)
        {
            var result = new List<GetPaymentsReturnData>
            {
                new GetPaymentsReturnData {
                    SrcDept = "EI",
                    FaAmount = "100.00",
                    FaDate = "2020/10/10",
                    DivertReq = "Y",
                    FeeInc = "Y",
                    MepSummons = "Y",
                    CourtSummons = "N",
                    DivertDate = "2020/10/10",
                    DivertAmt = "100.00",
                    FeeAmount = "25.00",
                    CrDate = "2020/10/10",
                    CrAmount = "100.00"
                },
                new GetPaymentsReturnData {
                    SrcDept = "RC",
                    FaAmount = "10.00",
                    FaDate = "2020/10/10",
                    DivertReq = "Y",
                    FeeInc = "Y",
                    MepSummons = "Y",
                    CourtSummons = "N",
                    DivertDate = "2020/10/10",
                    DivertAmt = "10.00",
                    FeeAmount = "25.00",
                    CrDate = "2020/10/10",
                    CrAmount = "10.00"
                }
            };

            return Task.FromResult<List<GetPaymentsReturnData>>(result);
        }

        public Task<CheckSinReturnData> CheckSinCount(CheckSinGetData data)
        {
            return Task.FromResult<CheckSinReturnData>(
                new CheckSinReturnData
                {
                    SinCount = 1
                });
        }

        public Task<GetSummonsReturnData> GetSummons(GetSummonsGetData data)
        {
            return Task.FromResult<GetSummonsReturnData>(
                new GetSummonsReturnData
                {
                    EnforcementCode = "AB01",
                    ControlCode = "123456",
                    SrcDept = "AB",
                    EnfSrvNme = "string",
                    DebtorId = "ABC1234A",
                    ApplDbtrCnfrmdSin = "123456789",
                    Dbtr = "ABCD",
                    Crdtr = "string",
                    ApplSourceRfrNr = "string",
                    Status = "ACTIVE",
                    GarnIssue = "2020/10/10",
                    EnfOffOrgCd = "string",
                    CourtCd = "N",
                    GarnEffective = "2020/10/10",
                    Arrears = "1234.56",
                    PayPer = "MONTHLY",
                    Periodic = "123.45",
                    Variation = "string",
                    Cancel = " ",
                    Suspend = " ",
                    Expired = " ",
                    Satisfied = " ",
                    LmpOwed = "123.45",
                    FeeOwed = "12.34",
                    PerOwed = "123.45",
                    LastCalc = "2020/10/10",
                    HldbCtgCd = "1",
                    HldbDefaultValue = "12"
                });
        }
    }
}
