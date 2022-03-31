using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBLicenceDenialResponse : DBbase, ILicenceDenialResponseRepository
    {
        public DBLicenceDenialResponse(IDBTools mainDB) : base(mainDB)
        {

        }

        //public DataList<LicenceDenialResponseData> GetLicenceDenialResponseForApplication(string applEnfSrvCd, string applCtrlCd)
        //{
        //    var parameters = new Dictionary<string, object>
        //        {
        //            {"EnfSrv_Cd", applEnfSrvCd },
        //            {"CtrlCd", applCtrlCd }
        //        };

        //    var data = MainDB.GetDataFromStoredProc<LicenceDenialResponseData>("LicenceDenialResultsGetLicenceDenial", parameters, FillLicenceDenialResultDataFromReader);

        //    return new DataList<LicenceDenialResponseData>(data, MainDB.LastError);
        //}

        public LicenceDenialResponseData GetLastResponseData(string applEnfSrvCd, string applCtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"EnfSrv_Cd", applEnfSrvCd },
                    {"CtrlCd", applCtrlCd }
                };

            var data = MainDB.GetDataFromStoredProc<LicenceDenialResponseData>("GetLastLicenseResponse", parameters, FillLicenceDenialResultDataFromReader);

            return data.FirstOrDefault();
        }

        /*
         
         Public Function GetLastLicRsp(ByVal applicationEnforcementServiceCode As String, ByVal applicationControlCode As String) As Common.LicenseResponseData
        Dim licenceData As New Common.LicenseResponseData

        Dim command As New SqlClient.SqlCommand
        With command
            .Connection = New SqlClient.SqlConnection(_connectionString)
            .CommandText = "GetLastLicenseResponse"
            .CommandType = CommandType.StoredProcedure

            'Add the parameters to the SqlCommand object.
            .Parameters.Add("@Appl_EnfSrv_Cd", SqlDbType.Char)
            .Parameters.Add("@Appl_CtrlCd", SqlDbType.Char)

            'Assign the parameters.
            .Parameters("@Appl_EnfSrv_Cd").Value = IIf(applicationEnforcementServiceCode Is Nothing, _
                                                       DBNull.Value, applicationEnforcementServiceCode)
            .Parameters("@Appl_CtrlCd").Value = IIf(applicationControlCode Is Nothing, _
                                                    DBNull.Value, applicationControlCode)
        End With

        With New SqlClient.SqlDataAdapter(command)
            .Fill(licenceData.LicRsp)
        End With

        command.Connection.Close()

        Return licenceData
    End Function
         */

        private void FillLicenceDenialResultDataFromReader(IDBHelperReader rdr, LicenceDenialResponseData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.LicRsp_Rcpt_Dte = (DateTime)rdr["LicRsp_Rcpt_Dte"];
            data.LicRsp_SeqNr = (short)rdr["LicRsp_SeqNr"];
            data.RqstStat_Cd = (short)rdr["RqstStat_Cd"];
            data.LicRsp_Comments = rdr["LicRsp_Comments"] as string; // can be null 
            if (rdr.ColumnExists("LicRspFilename"))
                data.LicRspFilename = rdr["LicRspFilename"] as string; // can be null 
            if (rdr.ColumnExists("LicRspType"))
                data.LicRspType = rdr["LicRspType"] as string; // can be null 
            if (rdr.ColumnExists("LicRspSource_RefNo"))
                data.LicRspSource_RefNo = rdr["LicRspSource_RefNo"] as string; // can be null 
            if (rdr.ColumnExists("LicRsp_RcptViewed_Ind"))
                data.LicRsp_RcptViewed_Ind = (bool)rdr["LicRsp_RcptViewed_Ind"];
            if (rdr.ColumnExists("LicRsp_RcptViewed_Date"))
                data.LicRsp_RcptViewed_Date = rdr["LicRsp_RcptViewed_Date"] as DateTime?; // can be null 
        }
    }
}
