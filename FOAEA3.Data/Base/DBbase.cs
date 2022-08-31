using DBHelper;

namespace FOAEA3.Data.Base
{
    public class DBbase
    {
        public readonly IDBToolsAsync MainDB;

        public string CurrentSubmitter 
        {
            get => MainDB.Submitter;
            set => MainDB.Submitter = value;
        }

        public string UserId
        {
            get => MainDB.UserId;
            set => MainDB.UserId = value;
        }

        public DBbase(IDBToolsAsync mainDB)
        {
            MainDB = mainDB;
        }

    }
}
