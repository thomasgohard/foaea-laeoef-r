using System.Threading.Tasks;

namespace Outgoing.FileCreator.Fed.Tracing
{
    class Program
    {
        static async Task Main(string[] args)
        {

            //var values = new Dictionary<string, string>
            //{
            //    {"FIRSTNAME", "Test Person"},
            //    {"30000", "100.00"} // BASC-PRSNL-AMT
            //};

            //string template = @"C:\CRATaxForms\2022\5006-R.pdf";
            //string destination = @"C:\CRATaxForms\Processed\Ontario2022T1.PDF";

            ////var missingValues = PdfHelper.FillPdf(template, destination, values);

            //(var data, var missingValues) = PdfHelper.FillPdf(template, values);

            //using (var fileStream = File.Create(destination))
            //{
            //    data.Seek(0, SeekOrigin.Begin);
            //    data.CopyTo(fileStream);
            //}

            //foreach (var value in missingValues)
            //{
            //    Console.WriteLine("Missing value: " + value);
            //}

            await OutgoingFileCreatorFedTracing.Run(args);
        }
    }
}
