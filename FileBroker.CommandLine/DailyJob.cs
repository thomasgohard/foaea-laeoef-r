using FileBroker.Model.Interfaces;

namespace FileBroker.CommandLine
{
    internal class DailyJob
    {
        private IFileTableRepository FileTable { get; }

        public DailyJob(IFileTableRepository fileTable)
        {
            FileTable = fileTable;
        }

        public async Task RunAsync()
        {
            // Outgoing.FileCreator.MEP
            // Outgoing.FileCreator.Fed.SIN
            // Outgoing.FileCreator.Fed.Tracing
            // Outgoing.FileCreator.Fed.LicenceDenial
        }
    }
}
