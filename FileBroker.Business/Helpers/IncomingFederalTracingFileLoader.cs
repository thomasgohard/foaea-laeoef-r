namespace FileBroker.Business.Helpers;

public class IncomingFederalTracingFileLoader
{
    private IFlatFileSpecificationRepository FlatFileSpecs { get; }
    private int ProcessId { get; }

    public IncomingFederalTracingFileLoader(IFlatFileSpecificationRepository flatFileSpecs, int processId)
    {
        FlatFileSpecs = flatFileSpecs;
        ProcessId = processId;
    }

    public async Task FillTracingFileDataFromFlatFileAsync(FedTracingFileBase fileData, string flatFile, List<string> errors)
    {
        var specs = await FlatFileSpecs.GetFlatFileSpecificationsForFileAsync(ProcessId);

        // extract data into object
        var flatFileLines = flatFile.Split("\n");
        foreach (var flatFileLine in flatFileLines)
        {
            string error = string.Empty;

            if (flatFileLine.Trim().Length > 2)
            {
                string recType = flatFileLine[..2];
                switch (recType)
                {
                    case "01":
                        FlatFileSpecHelper.ExtractRecTypeSingle(ref fileData.TRCIN01, flatFileLine, specs, recType, ref error);
                        break;

                    case "02":
                        FlatFileSpecHelper.ExtractRecTypeMultiple(fileData.TRCIN02, flatFileLine, specs, recType, ref error);
                        break;

                    case string rt when fileData.TRCINResidentials.ContainsKey(rt):
                        const string RESIDENTIAL_SPEC_CODE = "03";
                        FlatFileSpecHelper.ExtractRecTypeMultiple(fileData.TRCINResidentials[rt], flatFileLine, specs, RESIDENTIAL_SPEC_CODE, ref error);
                        break;

                    case string rt when fileData.TRCINEmployers.ContainsKey(rt):
                        const string EMPLOYERS_SPEC_CODE = "04";
                        FlatFileSpecHelper.ExtractRecTypeMultiple(fileData.TRCINEmployers[rt], flatFileLine, specs, EMPLOYERS_SPEC_CODE, ref error);
                        break;

                    case "99":
                        FlatFileSpecHelper.ExtractRecTypeSingle(ref fileData.TRCIN99, flatFileLine, specs, recType, ref error);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(error))
                errors.Add(error);

        }

    }

}
