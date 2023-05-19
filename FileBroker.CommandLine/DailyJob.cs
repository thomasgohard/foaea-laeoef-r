using Outgoing.FileCreator.Fed.LicenceDenial;
using Outgoing.FileCreator.Fed.SIN;
using Outgoing.FileCreator.Fed.Tracing;
using Outgoing.FileCreator.MEP;

namespace FileBroker.CommandLine
{
    internal class DailyJob
    {
        public static async Task Run()
        {
            await OutgoingFileCreatorMEP.Run();
            await OutgoingFileCreatorFedSIN.Run();
            await OutgoingFileCreatorFedTracing.Run();
            await OutgoingFileCreatorFedLicenceDenial.Run();
        }
    }
}
