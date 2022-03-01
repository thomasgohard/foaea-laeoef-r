using DBHelper;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace FileBroker.Business.Tests
{
    public class InMemoryTraceResponse : ITraceResponseRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public MessageDataList Messages => throw new NotImplementedException();

        public void DeleteCancelledApplicationTraceResponseData(string applEnfSrvCd, string applCtrlCd, string enfSrvCd)
        {
            throw new NotImplementedException();
        }

        public DataList<TraceResponseData> GetTraceResponseForApplication(string applEnfSrvCd, string applCtrlCd, bool checkCycle = false)
        {
            throw new NotImplementedException();
        }

        public void InsertBulkData(List<TraceResponseData> responseData)
        {
            //var db = new DBTools("");

            //var newDataTable = db.CreateAndFillDataTable<TraceResponseData>(responseData);

            //int count = newDataTable.Rows.Count;
        }

        public void MarkResponsesAsViewed(string enfService)
        {
            throw new NotImplementedException();
        }
    }
}
