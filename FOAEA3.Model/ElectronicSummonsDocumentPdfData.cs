namespace FOAEA3.Model
{
    public class ElectronicSummonsDocumentPdfData
    {
        public int PDFid { get; set; }
        public int ZipID { get; set; }
        public string PDFName { get; set; }
        public string EnfSrv { get; set; }
        public string Ctrl { get; set; }

        public MessageDataList Messages { get; set; }

        public ElectronicSummonsDocumentPdfData()
        {
            Messages = new MessageDataList();
        }
    }
}
