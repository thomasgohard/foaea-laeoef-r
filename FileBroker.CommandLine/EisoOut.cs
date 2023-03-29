using Outgoing.FileCreator.Fed.Interception;

namespace FileBroker.CommandLine
{
    internal static class EisoOut
    {
        public static async Task RunCRA()
        {
            await OutgoingFileCreatorFedInterception.RunCRA();
        }

        public static async Task RunEI(bool skipChecks = false)
        {
            await OutgoingFileCreatorFedInterception.RunEI(skipChecks: skipChecks);
        }

        public static async Task RunCPP()
        {
            await OutgoingFileCreatorFedInterception.RunCPP();
        }        
    }
}
