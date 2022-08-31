using FOAEA3.Model.Structs;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IPostalCodeRepository
    {
        Task<(bool, string, PostalCodeFlag)> ValidatePostalCodeAsync(string postalCode, string provinceCode, string cityName);
    }
}
