using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IFoaEventsRepository : IMessageList
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        Task<FoaEventDataDictionary> GetAllFoaMessagesAsync();
    }
}