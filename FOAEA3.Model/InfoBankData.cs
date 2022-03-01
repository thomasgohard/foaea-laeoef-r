using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOAEA3.Model
{
    public class InfoBankData
    {
        public string InfoBank_Cd { get; set; }
        public string InfoBank_Txt_E { get; set; }
        public string InfoBank_Txt_F { get; set; }
        public string Prv_Cd { get; set; }
        public string ActvSt_Cd { get; set; }

        public string Description
        {
            get => LanguageHelper.IsEnglish() ? InfoBank_Txt_E : InfoBank_Txt_F;
        }
    }
}
