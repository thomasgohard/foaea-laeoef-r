using DBHelper;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileBroker.Data.DB
{
    public class DBUser : IUserRepository
    {
        private IDBToolsAsync MainDB { get; }

        public DBUser(IDBToolsAsync mainDB)
        {
            MainDB = mainDB;
        }

        public async Task<UserData> GetUserByNameAsync(string userName)
        {
            var parameters = new Dictionary<string, object>
            {
                { "UserName", userName }
            };

            var data = await MainDB.GetDataFromStoredProcAsync<UserData>("User_SelectByUserName", parameters, FillUserDataFromReader);
            return data.FirstOrDefault();
        }

        public async Task<UserData> GetUserByIdAsync(int userId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "UserId", userId }
            };

            var data = await MainDB.GetDataFromStoredProcAsync<UserData>("User_Select", parameters, FillUserDataFromReader);
            return data.FirstOrDefault();
        }

        private void FillUserDataFromReader(IDBHelperReader rdr, UserData data)
        {
            data.UserId = (int)rdr["UserId"];
            data.UserName = rdr["UserName"] as string;
            data.EncryptedPassword = rdr["EncryptedPassword"] as string;
            data.PasswordSalt = rdr["PasswordSalt"] as string;
            data.RefreshToken =  (byte?[])rdr["RefreshToken"]; // can be null 
            data.RefreshTokenExpiration = (DateTime?)rdr["RefreshTokenExpiration"]; // can be null 
            data.SecurityRole = rdr["SecurityRole"] as string;
            data.EmailAddress = rdr["EmailAddress"] as string;
            data.IsActive = (bool)rdr["IsActive"];
            data.LastModified = (DateTime)rdr["LastModified"];
        }
    }
}
