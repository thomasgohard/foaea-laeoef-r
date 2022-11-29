using DBHelper;
using FileBroker.Business.Helpers;
using FileBroker.Business.Tests.InMemory;
using FileBroker.Data.DB;
using FileBroker.Model;
using FOAEA3.Data.DB;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileBroker.Business.Tests
{
    public class IncomingFederalTracingManagerTests
    {
        [Fact]
        public async Task ExtractNETPTracingDataFromFlatFile_Test1()
        {
            // Arrange

            var fileTable = new InMemoryFileTable();
            var messageBrokerDB = new DBToolsAsync("Server=%FOAEA_DB_SERVER%;Database=FoaeaMessageBroker;Integrated Security=SSPI;Trust Server Certificate=true;"
                                            .ReplaceVariablesWithEnvironmentValues());
            var flatFileSpecs = new DBFlatFileSpecification(messageBrokerDB);
            var processId = (await fileTable.GetFileTableDataForFileNameAsync("EI3STSIT")).PrcId;
            var fileLoader = new IncomingFederalTracingFileLoader(flatFileSpecs, processId);

            string fullPathFileName = @"TestDataFiles\EI3STSIT.000001";
            //string flatFileName = "EI3STSIT.000001";
            string flatFile = "";
            using (var streamReader = new StreamReader(fullPathFileName, Encoding.UTF8))
            {
                flatFile = streamReader.ReadToEnd();
            }

            // Act

            var netpTracingData = new FedTracingFileBase();
            netpTracingData.AddEmployerRecTypes("80", "81");
            var errors = new List<string>();
            await fileLoader.FillTracingFileDataFromFlatFileAsync(netpTracingData, flatFile, errors);

            // Assert

            if (errors.Count > 0)
            {
                foreach (var error in errors)
                    Assert.True(false, error);
            }
            Assert.Equal<int>(1, netpTracingData.TRCIN01.Cycle);
            Assert.Equal("D62990", netpTracingData.TRCIN02[0].dat_Appl_CtrlCd);

        }

        [Fact]
        public async Task ExtractCRATracingDataFromFlatFile_Test1()
        {
            // Arrange

            var fileTable = new InMemoryFileTable();
            var messageBrokerDB = new DBToolsAsync("Server=%FOAEA_DB_SERVER%;Database=FoaeaMessageBroker;Integrated Security=SSPI;Trust Server Certificate=true;"
                                            .ReplaceVariablesWithEnvironmentValues());
            var flatFileSpecs = new DBFlatFileSpecification(messageBrokerDB);
            var processId = (await fileTable.GetFileTableDataForFileNameAsync("RC3STSIT")).PrcId;
            var fileLoader = new IncomingFederalTracingFileLoader(flatFileSpecs, processId);

            string fullPathFileName = @"TestDataFiles\RC3STSIT.001";
            //string flatFileName = "RC3STSIT.001";
            string flatFile = "";
            using (var streamReader = new StreamReader(fullPathFileName, Encoding.UTF8))
                flatFile = streamReader.ReadToEnd();

            // Act

            var craTracingData = new FedTracingFileBase();
            craTracingData.AddResidentialRecTypes("03", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15",
                                                  "16", "17", "18", "19", "20", "21");
            craTracingData.AddEmployerRecTypes("04", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32",
                                               "33", "34", "35", "36");
            var errors = new List<string>();
            await fileLoader.FillTracingFileDataFromFlatFileAsync(craTracingData, flatFile, errors);

            // Assert

            if (errors.Count > 0)
            {
                foreach (var error in errors)
                    Assert.True(false, error);
            }
            Assert.Equal<int>(1, craTracingData.TRCIN01.Cycle);
            Assert.Equal<int>(craTracingData.TRCIN02.Count, craTracingData.TRCIN99.ResponseCnt);
        }

        [Fact]
        public async Task ExtractEITracingDataFromFlatFile_Test1()
        {
            // Arrange

            var fileTable = new InMemoryFileTable();
            var messageBrokerDB = new DBToolsAsync("Server=%FOAEA_DB_SERVER%;Database=FoaeaMessageBroker;Integrated Security=SSPI;Trust Server Certificate=true;"
                                        .ReplaceVariablesWithEnvironmentValues());
            var flatFileSpecs = new DBFlatFileSpecification(messageBrokerDB);
            var processId = (await fileTable.GetFileTableDataForFileNameAsync("HR3STSIT")).PrcId;
            var fileLoader = new IncomingFederalTracingFileLoader(flatFileSpecs, processId);

            string fullPathFileName = @"TestDataFiles\HR3STSIT.000001";
            //string flatFileName = "HR3STSIT.000001";
            string flatFile = "";
            using (var streamReader = new StreamReader(fullPathFileName, Encoding.UTF8))
            {
                flatFile = streamReader.ReadToEnd();
            }

            // Act

            var eiTracingData = new FedTracingFileBase();
            eiTracingData.AddResidentialRecTypes("03");
            eiTracingData.AddEmployerRecTypes("04");
            var errors = new List<string>();
            await fileLoader.FillTracingFileDataFromFlatFileAsync(eiTracingData, flatFile, errors);

            // Assert

            if (errors.Count > 0)
            {
                foreach (var error in errors)
                    Assert.True(false, error);
            }
            Assert.Equal<int>(1, eiTracingData.TRCIN01.Cycle);
            Assert.Equal<int>(eiTracingData.TRCIN02.Count, eiTracingData.TRCIN99.ResponseCnt);
        }

        //[Fact]
        //public async Task CombineCRADataIntoTracingResponses_Test1()
        //{
        //    // Arrange

        //    var fileTable = new InMemoryFileTable();
        //    var messageBrokerDB = new DBToolsAsync("Server=%FOAEA_DB_SERVER%;Database=FoaeaMessageBroker;Integrated Security=SSPI;Trust Server Certificate=true;"
        //                                .ReplaceVariablesWithEnvironmentValues());
        //    var foaeaDB = new DBToolsAsync("Server=%FOAEA_DB_SERVER%;Database=FOAEA_DEV;Integrated Security=SSPI;Trust Server Certificate=true;"
        //                                .ReplaceVariablesWithEnvironmentValues());
        //    var flatFileSpecs = new DBFlatFileSpecification(messageBrokerDB);
        //    var tracingDB = new DBTracing(foaeaDB);

        //    var processId = (await fileTable.GetFileTableDataForFileNameAsync("RC3STSIT")).PrcId;
        //    var fileLoader = new IncomingFederalTracingFileLoader(flatFileSpecs, processId);

        //    var craTracingData = new FedTracingFileBase();
        //    craTracingData.AddResidentialRecTypes("03", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15",
        //                                          "16", "17", "18", "19", "20", "21");
        //    craTracingData.AddEmployerRecTypes("04", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32",
        //                                       "33", "34", "35", "36");

        //    string fullPathFileName = @"TestDataFiles\RC3STSIT.001";
        //    //string flatFileName = "RC3STSIT.001";
        //    string flatFile = "";

        //    // Act

        //    using (var streamReader = new StreamReader(fullPathFileName, Encoding.UTF8))
        //        flatFile = streamReader.ReadToEnd();

        //    var errors = new List<string>();
        //    await fileLoader.FillTracingFileDataFromFlatFileAsync(craTracingData, flatFile, errors);

        //    var cycles = await tracingDB.GetTraceCycleQuantityDataAsync("RC01", Path.GetExtension("RC3STSIT.356"));

        //    var traceResponses = IncomingFederalTracingResponse.GenerateFromFileData(craTracingData, "RC01", cycles, ref errors);

        //    SaveTraceResponsesToFile(traceResponses);

        //    // Assert

        //    Assert.Equal<int>(339, traceResponses.Count);
        //    Assert.Equal<int>(craTracingData.TRCIN02.Count, craTracingData.TRCIN99.ResponseCnt);
        //}

        private static void SaveTraceResponsesToFile(List<TraceResponseData> traceResponses)
        {
            string outputFile = @"C:\Work\TraceResponses.txt";

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            using var fs = new StreamWriter(outputFile, false);
            foreach (var item in traceResponses)
                fs.WriteLine($"{item.Appl_EnfSrv_Cd}\t{item.Appl_CtrlCd}\t{item.EnfSrv_Cd}\t{item.TrcRsp_Rcpt_Dte}\t{item.TrcRsp_SeqNr}\t" +
                    $"{item.TrcRsp_EmplNme}\t{item.TrcRsp_EmplNme1}\t{item.TrcSt_Cd}\t{item.TrcRsp_Addr_Ln}\t{item.TrcRsp_Addr_Ln1}\t" +
                    $"{item.TrcRsp_Addr_CityNme}\t{item.TrcRsp_Addr_PrvCd}\t{item.TrcRsp_Addr_CtryCd}\t{item.TrcRsp_Addr_PCd}\t{item.TrcRsp_Addr_LstUpdte}\t" +
                    $"{item.AddrTyp_Cd}\t{item.TrcRsp_SubmViewed_Ind}\t{item.TrcRsp_RcptViewed_Ind}\t{item.TrcRsp_SubmAddrUsed_Ind}\t{item.TrcRsp_SubmAddrHasValue_Ind}\t" +
                    $"{item.TrcRsp_Trace_CyclNr}\t{item.ActvSt_Cd}\t{item.Prcs_RecType}\t{item.TrcRsp_RcptViewed_Dte}"
                    );


        }
    }

}
