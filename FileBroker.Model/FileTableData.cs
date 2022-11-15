using System;

namespace FileBroker.Model
{
    public class FileTableData
    {
        public int PrcId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Cycle { get; set; }
        public bool Transform { get; set; }
        public string Meduim { get; set; }
        public string Address { get; set; }
        public string Path { get; set; }
        public int? Frequency { get; set; }
        public DateTime? Nextrun { get; set; }
        public string Category { get; set; }
        public bool? Active { get; set; }
        public bool IsXML { get; set; }
        public bool IsReg { get; set; }
        public bool UsePADRSource { get; set; }
        public DateTime? StartDate { get; set; }
        public short UseFixedTag { get; set; }
        public bool IsLoading { get; set; }
    }
}
