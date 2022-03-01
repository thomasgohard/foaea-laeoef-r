using FOAEA3.Model.Interfaces;
using FOAEA3.Resources;
using System.ComponentModel.DataAnnotations;

namespace FOAEA3.Model
{
    public class LoginData : IMessageList
    {
        [Display(Name = "USER_NAME_LABEL", ResourceType = typeof(LanguageResource))]
        public string UserName { get; set; }

        [Display(Name = "PASSWORD_LABEL", ResourceType = typeof(LanguageResource))]
        public string Password { get; set; }

        public MessageDataList Messages { get; set; }

        public LoginData()
        {
            Messages = new MessageDataList();
        }
    }
}
