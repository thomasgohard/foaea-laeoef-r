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

    public void FillSinFileDataFromFlatFile(FedSinFileBase fileData, string flatFile, ref List<string> errors)
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
                        SpecHelper.ExtractRecTypeSingle<FedSin_RecType01>(ref fileData.SININ01, flatFileLine, specs, recType, ref error);
                        break;

                    case "02":
                        SpecHelper.ExtractRecTypeMultiple<FedSin_RecType02>(fileData.SININ02, flatFileLine, specs, recType, ref error);
                        break;

                    case "99":
                        SpecHelper.ExtractRecTypeSingle<FedSin_RecType99>(ref fileData.SININ99, flatFileLine, specs, recType, ref error);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(error))
                errors.Add(error);

        }

    }
}
