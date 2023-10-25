namespace FileBroker.Business.Helpers;

public class IncomingFederalSinFileLoader
{
    private IFlatFileSpecificationRepository FlatFileSpecs { get; }
    private int ProcessId { get; }

    public IncomingFederalSinFileLoader(IFlatFileSpecificationRepository flatFileSpecs, int processId)
    {
        FlatFileSpecs = flatFileSpecs;
        ProcessId = processId;
    }

    public async Task FillSinFileDataFromFlatFile(FedSinFileBase fileData, string flatFile, List<string> errors)
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
                        FlatFileSpecHelper.ExtractRecTypeSingle(ref fileData.SININ01, flatFileLine, specs, recType, lineNumber, ref error);
                        break;

                    case "02":
                        FlatFileSpecHelper.ExtractRecTypeMultiple(fileData.SININ02, flatFileLine, specs, recType, lineNumber, ref error);
                        break;

                    case "99":
                        FlatFileSpecHelper.ExtractRecTypeSingle(ref fileData.SININ99, flatFileLine, specs, recType, lineNumber, ref error);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(error))
                errors.Add(error);

            lineNumber++;

        }

    }
}
