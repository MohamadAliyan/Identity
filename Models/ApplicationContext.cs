using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Identity.Models
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser,ApplicationRole,int>
    {
        public ApplicationContext(DbContextOptions options)
            : base(options)
        {
        }
 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
