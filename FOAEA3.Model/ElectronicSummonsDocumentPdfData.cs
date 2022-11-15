namespace FOAEA3.Model
{
    public class ElectronicSummonsDocumentPdfData
    {
        public int PDFid { get; set; }
        public int ZipID { get; set; }
        public string PDFName { get; set; }
        public string EnfSrv { get; set; }
        public string Ctrl { get; set; }
    }

    /*
        data.PDFid = (int)rdr["PDFid"];
        data.ZipID = (int)rdr["ZipID"];
        data.PDFName = rdr["PDFName"] as string;
        data.EnfSrv = rdr["EnfSrv"] as string;
        data.Ctrl = rdr["Ctrl"] as string;  
     */
}
