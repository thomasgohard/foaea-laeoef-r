using FOAEA3.Model.Base;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IDocumentTypeRepository : IMessageList
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<DataList<DocumentTypeData>> GetDocumentTypes();
    }
}
