using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model
{
    public class PasswordData
    {
        public string ConfirmationCode { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Initial { get; set; }
    }
}
