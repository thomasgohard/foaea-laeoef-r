using FOAEA3.Model.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FileBroker.Model
{
    public struct FedTracingFinancial_Header
    {
        public int Cycle;
        public DateTime FileDate;
    }

    public struct FedTracingFinancial_TraceResponse
    {
        public string Appl_EnfSrvCd;
        public string Appl_CtrlCd;
        public string SIN;
        public string SIN_XRef;
        public string ResponseCode;
        [DataMember(IsRequired = false)]
        public FedTracingFinancial_TaxResponse Tax_Response;
    }

    public struct FedTracingFinancial_TaxResponse
    {
        [DataMember(IsRequired = false)]
        [JsonConverter(typeof(SingleOrArrayConverter<FedTracingFinancial_TaxData>))]
        public List<FedTracingFinancial_TaxData> Tax_Data;
    }

    public struct FedTracingFinancial_TaxData
    {
        [JsonProperty("@year")]
        public string Year;
        [JsonProperty("@form")]
        public string Form;
        [DataMember(IsRequired = false)]
        [JsonConverter(typeof(SingleOrArrayConverter<FedTracingFinancial_Field>))]
        public List<FedTracingFinancial_Field> Field;
    }

    public struct FedTracingFinancial_Field
    {
        [JsonProperty("@name")]
        public string Name;
        [JsonProperty("#text")]
        public string Value;
    }

    public struct FedTracingFinancial_Footer
    {
        public int ResponseCount;
    }

    public struct FedTracingFinancial_CRATraceIn
    {
        public FedTracingFinancial_Header Header;
        [DataMember(IsRequired = false)]
        [JsonConverter(typeof(SingleOrArrayConverter<FedTracingFinancial_TraceResponse>))]
        public List<FedTracingFinancial_TraceResponse> TraceResponse;
        public FedTracingFinancial_Footer Footer;
    }

    public class FedTracingFinancialFileBase
    {
        public FedTracingFinancial_CRATraceIn CRATraceIn;
    }

}
