using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface IUserRepository
    {
        Task<UserData> GetUserByNameAsync(string userName);
        Task<UserData> GetUserByIdAsync(int userId);
    }

}
