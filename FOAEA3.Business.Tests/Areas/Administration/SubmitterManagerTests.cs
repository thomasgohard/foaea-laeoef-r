using FOAEA3.Business.Areas.Administration;
using TestData.TestDataBase;
using FOAEA3.Model.Enums;
using FOAEA3.Model;
using System;
using Xunit;
using System.Threading.Tasks;

namespace FOAEA3.Data.Tests.Areas.Administration
{
    public class SubmitterManagerTests
    {
        private SubmitterData CreateTestSubmitter(string EnfSrv_Cd, string EnfOff_City_LocCd)
        {
            return new SubmitterData
            {
                EnfSrv_Cd = EnfSrv_Cd,
                EnfOff_City_LocCd = EnfOff_City_LocCd,
                Subm_FrstNme = "TestFN",
                Subm_SurNme = "TestSN",
                Subm_Tel_AreaC = "123",
                Subm_TelNr = "123-4321",
                Subm_TelEx = "12345",
                Subm_EnfOffAuth_Ind = true,
                Subm_EnfSrvAuth_Ind = false,
                Subm_Intrc_AccsPrvCd = true,
                Subm_CourtUsr_Ind = false,
                Messages = new MessageDataList()
            };
        }

        private void ValidateResult(string expectedValue, SubmitterData newSubmitter)
        {
            if (newSubmitter.Messages.ContainsMessagesOfType(MessageType.Error))
            {
                string message = "Error messages: ";

                foreach (var error in newSubmitter.Messages.GetMessagesForType(MessageType.Error))
                {
                    if (error.Code != default)
                        message += $"\n  [Code:{error.Code}]{error.Field}: {error.Description}";
                    else
                        message += $"\n  {error.Field}: {error.Description}";
                }
                throw new Exception(message);
            }
            else
                Assert.Equal(expectedValue, newSubmitter.Subm_SubmCd);
        }

        [Fact]
        public async Task TestCreateRegularSubmitter()
        {

            SubmitterManager submitterManager = new SubmitterManager(new InMemory_Repositories());

            SubmitterData submitterData = CreateTestSubmitter("ON01", "DOW1");

            SubmitterData newSubmitter = await submitterManager.CreateSubmitterAsync(submitterData, string.Empty, false);

            ValidateResult("ON2D03", newSubmitter);

        }

        //[Trait("Category", "SkipWhenLiveUnitTesting")]
        [Fact]
        public async Task TestCreateFLASSubmitter()
        {

            SubmitterManager submitterManager = new SubmitterManager(new InMemory_Repositories());

            SubmitterData submitterData = CreateTestSubmitter("FO01", "OTT1");

            SubmitterData newSubmitter = await submitterManager.CreateSubmitterAsync(submitterData, "DS", false);

            ValidateResult("FO2XDS", newSubmitter);

        }

        [Fact]
        public async Task TestCreateCourtSubmitter()
        {

            SubmitterManager submitterManager = new SubmitterManager(new InMemory_Repositories());

            SubmitterData submitterData = CreateTestSubmitter("ON02", "ON15");
            submitterData.Subm_CourtUsr_Ind = true;

            SubmitterData newSubmitter = await submitterManager.CreateSubmitterAsync(submitterData, string.Empty, false);

            ValidateResult("ONC003", newSubmitter);

        }

    }
}
