using System.Threading.Tasks;

namespace Outgoing.FileCreator.Fed.SIN;

internal class Program
{
    static async Task Main(string[] args)
    {
        await OutgoingFileCreatorFedSIN.Run(args);
    }

}
