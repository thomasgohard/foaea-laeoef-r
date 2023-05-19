using System;

namespace FileBroker.Model
{
    public class UserData
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EncryptedPassword { get; set; }
        public string PasswordSalt { get; set; }
        public string SecurityRole { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastModified { get; set; }
    }
}
