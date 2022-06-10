using FOAEA3.Model.Base;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IDocumentTypeRepository : IMessageList
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        DataList<DocumentTypeData> GetDocumentTypes();
    }
}
