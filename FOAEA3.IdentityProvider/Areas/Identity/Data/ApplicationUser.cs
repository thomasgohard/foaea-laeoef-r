using Microsoft.AspNetCore.Identity;

namespace FOAEA3.IdentityManager.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public int? SubjectId { get; set; }
    }
}
