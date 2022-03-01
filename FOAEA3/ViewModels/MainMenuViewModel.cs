using FOAEA3.Business.Security;
using FOAEA3.Model;

namespace FOAEA3.ViewModels
{
    public class MainMenuViewModel
    {
        public UserSecurity UserSecurityData { get; set; }
        public SubmitterProfileData UserProfileData { get; set; }
        public string SubmitterCode { get; set; }
    }
}
