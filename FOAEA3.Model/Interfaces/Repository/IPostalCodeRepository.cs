using FOAEA3.Model.Structs;

namespace FOAEA3.Model.Interfaces
{
    public interface IPostalCodeRepository
    {
        bool ValidatePostalCode(string postalCode, string provinceCode, string cityName, 
                                out string validProvCode, out PostalCodeFlag validFlags);
    }
}
