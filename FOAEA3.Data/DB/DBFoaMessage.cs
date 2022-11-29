using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Exceptions;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBFoaMessage : DBbase, IFoaEventsRepository
    {
        public MessageDataList Messages { get; set; }

        private class FoaMessageData
        {
            public int Error { get; set; }
            public short Severity { get; set; }
            public short Dlevel { get; set; }
            public string Description { get; set; }
            public short? MsgLangId { get; set; }
        }

        public DBFoaMessage(IDBToolsAsync mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public async Task<FoaEventDataDictionary> GetAllFoaMessagesAsync()
        {
            var result = new FoaEventDataDictionary();

            string connStr = MainDB.ConnectionString;
            try
            {
                var data = await MainDB.GetAllDataAsync<FoaMessageData>("FoaMessages", FillFoaMessageDataFromReader);

                if (!string.IsNullOrEmpty(MainDB.LastError))
                    Messages.AddSystemError(MainDB.LastError);

                foreach (var eventData in data)
                {
                    EventCode eventCode = (EventCode)eventData.Error;
                    if (!result.ContainsKey(eventCode))
                    {
                        var newEventData = new FoaEventData
                        {
                            Error = eventData.Error,
                            Dlevel = eventData.Dlevel,
                            Severity = eventData.Severity,
                        };

                        var thisCode = (EventCode)eventData.Error;

                        result.FoaEvents.TryAdd(((int)thisCode).ToString(), newEventData);
                    }

                    if (eventData.MsgLangId == 1033)
                        result[eventCode].Description_e = eventData.Description;
                    else
                        result[eventCode].Description_f = eventData.Description;
                }

                return result;
            }
            catch (Exception e)
            {
                if ((Thread.CurrentPrincipal is not null) && (Thread.CurrentPrincipal.Identity is not null))
                    e.Data.Add("user", Thread.CurrentPrincipal.Identity.Name);
                e.Data.Add("connection", connStr);
                throw new ReferenceDataException("Could not load FoaMessages! ", e);
            }
        }

        private void FillFoaMessageDataFromReader(IDBHelperReader rdr, FoaMessageData data)
        {
            data.Error = (int)rdr["error"];
            data.Severity = (short)rdr["severity"];
            data.Dlevel = (short)rdr["dlevel"];
            data.Description = rdr["description"] as string;
            data.MsgLangId = rdr["msglangid"] as short?; // can be null
        }

    }
}
