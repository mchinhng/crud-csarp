using Microsoft.EntityFrameworkCore;
using UserInfo.Web.Models.Entities;

namespace UserInfo.Web.Data
{
    public class ApplicationDbContext: DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }

    }
}