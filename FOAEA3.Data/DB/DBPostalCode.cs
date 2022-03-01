using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Structs;
using System.Collections.Generic;

namespace FOAEA3.Data.DB
{
    internal class DBPostalCode : DBbase, IPostalCodeRepository
    {
        public DBPostalCode(IDBTools mainDB) : base(mainDB)
        {

        }

        public bool ValidatePostalCode(string postalCode, string provinceCode, string cityName,
                                       out string validProvCode,
                                       out PostalCodeFlag validFlags)
        {
            var parameters = new Dictionary<string, object>
            {
                { "PostalCode", postalCode },
                { "ProvinceCode", provinceCode },
                { "CityName", cityName }
            };

            var returnParameters = new Dictionary<string, string>
            {
                {"ValidProvCode", "C2" },
                {"Results", "C3" }
            };

            var result = MainDB.GetDataFromStoredProcViaReturnParameters("fp_ValidatePostalCode", parameters, returnParameters);

            validProvCode = result["ValidProvCode"] as string;
            string validFlagString = result["Results"] as string;

            validFlags = new PostalCodeFlag
            {
                IsPostalCodeValid = validFlagString[0] == '1',
                IsProvinceValid = validFlagString[1] == '1',
                IsCityNameValid = validFlagString[2] == '1',
            };

            return validFlags.IsPostalCodeValid;
        }
    }
}
