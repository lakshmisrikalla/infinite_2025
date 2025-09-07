using System.Data.Entity;

namespace LTI.Models
{
    public class InsuranceDbContext : DbContext
    {
        public InsuranceDbContext() : base("DefaultConnection") { }

        public DbSet<LoginCredentials> LoginCredentials { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
