using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOAEA3.Model.Structs
{
    public readonly struct PostalCodeFlag
    {
        public bool IsPostalCodeValid { get; init; }
        public bool IsProvinceValid { get; init; }
        public bool IsCityNameValid { get; init; }
    }
}
