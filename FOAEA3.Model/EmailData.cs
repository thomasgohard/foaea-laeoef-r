using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model
{
    public class EmailData
    {
        public string Subject { get; set; }
        public string Recipient { get; set; }
        public string Body { get; set; }
        public int IsHTML { get; set; }
    }
}
