using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface INotificationRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task SendEmailAsync(string subject, string recipient, string body, int isHTML = 1);
        Task SendHtmlEmailAsync(string subject, string recipient, string body, string attachmentPath);
    }
}
