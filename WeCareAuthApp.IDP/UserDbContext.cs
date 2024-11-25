using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WeCareAuthApp.IDP
{
    public class UserDbContext : IdentityDbContext<User, Role, string>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }
    }
}
