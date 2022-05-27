using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;

namespace FOAEA3.Data.DB
{
    public class DBCountry : DBbase, ICountryRepository
    {
        public MessageDataList Messages { get; set; }

        public DBCountry(IDBTools mainDB) : base(mainDB)
        {
            Messages = new MessageDataList();
        }

        public DataList<CountryData> GetCountries()
        {
            var data = MainDB.GetAllData<CountryData>("Ctry", FillCountryDataFromReader);

            return new DataList<CountryData>(data, MainDB.LastError);
        }

        private void FillCountryDataFromReader(IDBHelperReader rdr, CountryData data)
        {
            data.Ctry_Cd = rdr["Ctry_Cd"] as string;
            data.Ctry_Txt_E = rdr["Ctry_Txt_E"] as string;
            data.Ctry_Txt_F = rdr["Ctry_Txt_F"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string; 
        }
    }
}
