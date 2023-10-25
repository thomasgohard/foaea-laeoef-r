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

    public async Task FillTracingFileDataFromFlatFile(FedTracingFileBase fileData, string flatFile, List<string> errors)
    {
        var specs = await FlatFileSpecs.GetFlatFileSpecificationsForFile(ProcessId);

        // extract data into object
        var flatFileLines = flatFile.Split("\n");
        int lineNumber = 1;
        foreach (var flatFileLine in flatFileLines)
        {
            string error = string.Empty;

            if (flatFileLine.Trim().Length > 2)
            {
                string recType = flatFileLine[..2];
                switch (recType)
                {
                    case "01":
                        FlatFileSpecHelper.ExtractRecTypeSingle(ref fileData.TRCIN01, flatFileLine, specs, recType, lineNumber, ref error);
                        break;

                    case "02":
                        FlatFileSpecHelper.ExtractRecTypeMultiple(fileData.TRCIN02, flatFileLine, specs, recType, lineNumber, ref error);
                        break;

                    case string rt when fileData.TRCINResidentials.ContainsKey(rt):
                        const string RESIDENTIAL_SPEC_CODE = "03";
                        FlatFileSpecHelper.ExtractRecTypeMultiple(fileData.TRCINResidentials[rt], flatFileLine, specs, RESIDENTIAL_SPEC_CODE, lineNumber, ref error);
                        break;

                    case string rt when fileData.TRCINEmployers.ContainsKey(rt):
                        if ((rt != "80") && (rt != "81"))
                        {
                            const string EMPLOYERS_SPEC_CODE = "04";
                            FlatFileSpecHelper.ExtractRecTypeMultiple(fileData.TRCINEmployers[rt], flatFileLine, specs, EMPLOYERS_SPEC_CODE, lineNumber, ref error);
                        }
                        else
                        {
                            FlatFileSpecHelper.ExtractRecTypeMultiple(fileData.TRCINEmployers[rt], flatFileLine, specs, rt, lineNumber, ref error);
                        }

                        break;

                    case "99":
                        FlatFileSpecHelper.ExtractRecTypeSingle(ref fileData.TRCIN99, flatFileLine, specs, recType, lineNumber, ref error);
                        break;
                }
                lineNumber++;
            }

            if (!string.IsNullOrEmpty(error))
                errors.Add(error);

        }

    }

}
