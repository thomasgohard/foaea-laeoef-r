using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces.Repository;
using FOAEA3.Model.Structs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBPostalCode : DBbase, IPostalCodeRepository
    {
        public DBPostalCode(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<(bool, string, PostalCodeFlag)> ValidatePostalCode(string postalCode, string provinceCode, string cityName)
        {
            PostalCodeFlag validFlags;

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

            var result = await MainDB.GetDataFromStoredProcViaReturnParametersAsync("fp_ValidatePostalCode", parameters, returnParameters);

            string validProvCode = result["ValidProvCode"] as string;
            string validFlagString = result["Results"] as string;

            if ((validFlagString is not null) && (validFlagString.Length >= 3))
                validFlags = new PostalCodeFlag
                {
                    IsPostalCodeValid = validFlagString[0] == '1',
                    IsProvinceValid = validFlagString[1] == '1',
                    IsCityNameValid = validFlagString[2] == '1',
                };
            else
                validFlags = new PostalCodeFlag
                {
                    IsPostalCodeValid = false,
                    IsProvinceValid = false,
                    IsCityNameValid = false,
                };

            return (validFlags.IsPostalCodeValid, validProvCode, validFlags);
        }
    }
}
