using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IFoaEventsRepository : IMessageList
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        Task<FoaEventDataDictionary> GetAllFoaMessagesAsync();
    }
}