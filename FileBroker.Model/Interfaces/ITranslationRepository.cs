using System.Collections.Generic;

namespace FileBroker.Model.Interfaces
{
    public interface ITranslationRepository
    {
        List<TranslationData> GetTranslations();
    }
}
