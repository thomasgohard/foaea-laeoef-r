using DBHelper;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using System.Collections.Generic;

namespace FileBroker.Data.DB
{
    public class DBTranslation : ITranslationRepository
    {
        private IDBTools MainDB { get; }

        public DBTranslation(IDBTools mainDB)
        {
            MainDB = mainDB;
        }

        public List<TranslationData> GetTranslations()
        {
            return MainDB.GetDataFromStoredProc<TranslationData>("", FillTranslationDataFromReader);
        }

        private void FillTranslationDataFromReader(IDBHelperReader rdr, TranslationData data)
        {
            data.TranslationId = (int)rdr["TranslationId"];
            data.EnglishText = rdr["EnglishText"] as string;
            data.FrenchText = rdr["FrenchText"] as string;
        }
    }
}
