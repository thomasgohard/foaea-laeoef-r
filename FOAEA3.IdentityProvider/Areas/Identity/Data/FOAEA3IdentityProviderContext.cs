using FOAEA3.IdentityManager.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FOAEA3.IdentityManager.Data;

public class FOAEA3IdentityManagerContext : IdentityDbContext<ApplicationUser>
{
    public FOAEA3IdentityManagerContext(DbContextOptions<FOAEA3IdentityManagerContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
