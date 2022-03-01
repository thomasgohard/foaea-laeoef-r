using System;
using System.Collections.Generic;
using System.Text;

namespace FileBroker.Model
{
    public class FlatFileSpecificationData
    {
        public short PrcsProcess_Cd { get; set; }
        public byte Val_Direction { get; set; }
        public string Val_FileSect { get; set; }
        public int Val_SortOrder { get; set; }
        public string Field_Name { get; set; }
        public int Val_Pos_Start { get; set; }
        public int Val_Pos_End { get; set; }
        public byte Val_Include { get; set; }
        public byte Val_Required { get; set; }
        public int? Val_Reas_Cd { get; set; }
        public string FormatDate { get; set; }
        public string PrcsType_Cd { get; set; }
        public byte Val_DfltApply { get; set; }
        public string Val_DfltVal { get; set; }
        public string ActvSt_Cd { get; set; }
    }
}
