using FileBroker.Model;
using FileBroker.Model.Interfaces;
using System.Collections.Generic;

namespace FileBroker.Business.Helpers
{
    public class IncomingFederalTracingFileLoader
    {
        private IFlatFileSpecificationRepository FlatFileSpecs { get; }
        private int ProcessId { get; }

        public IncomingFederalTracingFileLoader(IFlatFileSpecificationRepository flatFileSpecs, int processId)
        {
            FlatFileSpecs = flatFileSpecs;
            ProcessId = processId;
        }

        public void FillTracingFileDataFromFlatFile(FedTracingFileBase fileData, string flatFile, ref List<string> errors)
        {
            var specs = FlatFileSpecs.GetFlatFileSpecificationsForFile(ProcessId);

            // extract data into object
            var flatFileLines = flatFile.Split("\n");
            foreach (var flatFileLine in flatFileLines)
            {
                string error = string.Empty;

                if (flatFileLine.Trim().Length > 2)
                {
                    string recType = flatFileLine.Substring(0, 2);
                    switch (recType)
                    {
                        case "01":
                            SpecHelper.ExtractRecTypeSingle<FedTracing_RecType01>(ref fileData.TRCIN01, flatFileLine, specs, recType, ref error);
                            break;

                        case "02":
                            SpecHelper.ExtractRecTypeMultiple<FedTracing_RecType02>(fileData.TRCIN02, flatFileLine, specs, recType, ref error);
                            break;

                        case string rt when fileData.TRCINResidentials.ContainsKey(rt):
                            const string RESIDENTIAL_SPEC_CODE = "03";
                            SpecHelper.ExtractRecTypeMultiple<FedTracing_RecTypeResidential>(fileData.TRCINResidentials[rt], flatFileLine, specs, RESIDENTIAL_SPEC_CODE, ref error);
                            break;

                        case string rt when fileData.TRCINEmployers.ContainsKey(rt):
                            const string EMPLOYERS_SPEC_CODE = "04";
                            SpecHelper.ExtractRecTypeMultiple<FedTracing_RecTypeEmployer>(fileData.TRCINEmployers[rt], flatFileLine, specs, EMPLOYERS_SPEC_CODE, ref error);
                            break;

                        case "99":
                            SpecHelper.ExtractRecTypeSingle<FedTracing_RecType99>(ref fileData.TRCIN99, flatFileLine, specs, recType, ref error);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(error))
                    errors.Add(error);

            }

        }

    }
}
