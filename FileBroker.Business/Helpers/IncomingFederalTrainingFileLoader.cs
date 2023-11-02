namespace FileBroker.Business.Helpers
{
    public class IncomingFederalTrainingFileLoader
    {
        private IFlatFileSpecificationRepository FlatFileSpecs { get; }
        private int ProcessId { get; }

        public IncomingFederalTrainingFileLoader(IFlatFileSpecificationRepository flatFileSpecs, int processId)
        {
            FlatFileSpecs = flatFileSpecs;
            ProcessId = processId;
        }

        public async Task FillFederalTrainingFileDataFromFlatFile(FedInterceptionTrainingBase fileData, string flatFile,
                                                                       List<string> errors)
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
                            FlatFileSpecHelper.ExtractRecTypeSingle(ref fileData.TRIN01, flatFileLine, specs, recType, lineNumber, ref error);
                            break;

                        case "02":
                            FlatFileSpecHelper.ExtractRecTypeMultiple(fileData.TRIN02, flatFileLine, specs, recType, lineNumber, ref error);
                            break;

                        case "99":
                            FlatFileSpecHelper.ExtractRecTypeSingle(ref fileData.TRIN99, flatFileLine, specs, recType, lineNumber, ref error);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(error))
                    errors.Add(error);

                lineNumber++;

            }

        }
    }
}
