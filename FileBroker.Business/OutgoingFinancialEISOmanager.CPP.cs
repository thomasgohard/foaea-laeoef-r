namespace FileBroker.Business
{
    public partial class OutgoingFinancialEISOmanager
    {
        public async Task<string> CreateCPPfile(string fileBaseName, List<string> errors)
        {
            var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileBaseName);

            string newCycle = (fileTableData.Cycle + 1).ToString();
            string newFilePath = fileTableData.Path.AppendToPath(fileTableData.Name + newCycle, isFileName: true);

            if (File.Exists(newFilePath))
            {
                errors.Add("** Error: File Already Exists");
                return "";
            }

            return newFilePath;
        }
    }
}
