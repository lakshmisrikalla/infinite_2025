using System.Data.Entity;

namespace LTI.Models
{
    public class InsuranceDbContext : DbContext
    {
        public InsuranceDbContext() : base("DefaultConnection") { }

        public DbSet<LoginCredentials> LoginCredentials { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<UserPolicy> UserPolicies { get; set; }
        public DbSet<PremiumCalculation> PremiumCalculations { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        // Optional: if you want to test Client-side data
        public DbSet<Policy> Policies { get; set; }
        public DbSet<PolicyType> PolicyTypes { get; set; }
        //User-Policy Activities
        public DbSet<Renewal> Renewals { get; set; }
        public DbSet<Claim> Claims { get; set; }
        //public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasRequired(u => u.LoginCredentials).WithMany().HasForeignKey(u => u.LoginID);
        }
    }
}
