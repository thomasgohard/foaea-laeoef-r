using FOAEA3.Business.Areas.Application;
using TestData.TestDB;
using TestData.TestDataBase;
using FOAEA3.Data.Base;
using FOAEA3.Model.Enums;
using System;
using Xunit;
using FOAEA3.Model;

namespace FOAEA3.Data.Tests.Areas.Application
{
    public class TracingManagerTests
    {
        [Fact]
        public void TestProcessFinancialTermsVaried_17GenerateEvent50933()
        {
            ReferenceData.Instance().LoadFoaEvents(new InMemoryFoaEvents());

            var tracingManager = new TracingManager(new InMemory_Repositories(), new CustomConfig());

            tracingManager.SetNewStateTo(ApplicationState.FINANCIAL_TERMS_VARIED_17);

            if (tracingManager.EventManager.Events.Count > 0)
            {
                bool foundEvent = false;
                foreach (var eventData in tracingManager.EventManager.Events)
                    if (eventData.Event_Reas_Cd == EventCode.C50933_INVALID_OPERATION_FROM_THE_CURRENT_LIFE_STATE)
                    {
                        foundEvent = true;
                        break;
                    }

                Assert.True(foundEvent);
            }
            else
                throw new Exception("Missing event C50933_INVALID_OPERATION_FROM_THE_CURRENT_LIFE_STATE");

        }

        [Fact]
        public void TestProcessFinancialTermsVaried_17StateNotChanged()
        {
            ReferenceData.Instance().LoadFoaEvents(new InMemoryFoaEvents());

            var tracingManager = new TracingManager(new InMemory_Repositories(), new CustomConfig());

            ApplicationState oldState = tracingManager.TracingApplication.AppLiSt_Cd;
            tracingManager.SetNewStateTo(ApplicationState.FINANCIAL_TERMS_VARIED_17);
            ApplicationState newState = tracingManager.TracingApplication.AppLiSt_Cd;

            Assert.Equal(oldState, newState);

        }

    }
}
