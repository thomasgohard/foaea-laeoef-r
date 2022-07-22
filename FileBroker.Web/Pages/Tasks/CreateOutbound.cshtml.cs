using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace FileBroker.Web.Pages.Tasks
{
    public class CreateOutboundModel : PageModel
    {

        private IFileTableRepository FileTable { get; }
        private ApiConfig ApiConfig { get; }
        private IAPIBrokerHelper APIHelper { get; }

        public string InfoMessage { get; set; }

        public List<FileTableData> ActiveOutgoingProcesses { get; set; }

        public CreateOutboundModel(IFileTableRepository fileTable, IOptions<ApiConfig> apiConfig)
        {
            FileTable = fileTable;
            ApiConfig = apiConfig.Value;
            APIHelper = new APIBrokerHelper(currentSubmitter: "MSGBRO", currentUser: "MSGBRO");
        }

        public void OnGet()
        {
            ActiveOutgoingProcesses = FileTable.GetAllActive().Where(m => m.Type.ToLower() == "out").ToList();
        }

        public void OnPostCreateFiles(int[] selectedProcesses)
        {
            // Category.In("TRCAPPOUT", "LICAPPOUT", "STATAPPOUT", "TRCOUT", "SINOUT", "LICOUT")

            var processes = FileTable.GetAllActive().Where(m => m.Type.ToLower() == "out").ToList();

            InfoMessage = string.Empty;
            int lastItem = selectedProcesses.Last();
            foreach (int selectedProcess in selectedProcesses)
            {
                var thisProcess = processes.First(m => m.PrcId == selectedProcess);

                string fileName;
                if (thisProcess.IsXML)
                    fileName = $"{thisProcess.Name.Trim()}.{thisProcess.Cycle,6:D6}.XML";
                else
                    fileName = $"{thisProcess.Name.Trim()}.{thisProcess.Cycle,3:D3}";

                InfoMessage += $"Creating  {fileName} [{selectedProcess}] in {thisProcess.Path}";
                if (selectedProcess != lastItem)
                    InfoMessage += "; ";
            }
        }
    }
}
