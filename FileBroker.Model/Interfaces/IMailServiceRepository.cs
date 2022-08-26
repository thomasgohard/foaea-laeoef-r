using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface IMailServiceRepository
    {
        Task<string> SendEmailAsync(string subject, string recipients, string body, string attachmentPath = null);
    }
}
