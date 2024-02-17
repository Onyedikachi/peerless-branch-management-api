using Microsoft.EntityFrameworkCore;
using Retail.Branch.Core.Common;
using Retail.Branch.Core.Entities;


namespace Retail.Branch.Infrastructure
{

    public class BranchDataContext : DbContext
    {
        public BranchDataContext(DbContextOptions<BranchDataContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            
            this.ChangeTracker.LazyLoadingEnabled = false;


        }
        public DbSet<Core.Entities.Branch> Branches { get; set; }
        public DbSet<BranchRequest> BranchRequests { get; set; }
        public DbSet<BranchRequestLog> BranchRequestLogs { get; set; }
        public DbSet<BranchMember> BranchMembers { get; set; }
        public DbSet<BranchBranchRequest> BranchBranchRequest { get; set; } 

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created_At = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.Updated_At = DateTime.UtcNow;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Core.Entities.Branch>().HasKey(x => x.Id);  
            modelBuilder.Entity<Core.Entities.Branch>()
                .Property(c => c.Name).HasMaxLength(100);
            modelBuilder.Entity<Core.Entities.Branch>()
               .Property(c => c.Code).HasMaxLength(50);


            modelBuilder.Entity<Core.Entities.Branch>().HasData(new Core.Entities.Branch("Sterling Bank Head Office ", "HQ")
            {
                Id = Guid.Parse("f7c8559f-f718-4b16-aa59-41a5d6c718d2"),
                Created_By_Id = Guid.Empty.ToString(),
                Description = "Sterling Bank Head Quarters",
                IsLocked = true,
                Status = AppContants.APPROVED_BRANCH_STATUS,
                Created_At = new DateTime(2023, 8, 10, 7, 53, 27, 748, DateTimeKind.Utc).AddTicks(3612),
                Updated_At = new DateTime(2023, 8, 10, 7, 53, 27, 748, DateTimeKind.Utc).AddTicks(3612),
                State = "Lagos",
                PostalCode = "P.M.B. 12735",
                Number = "20",
                StreetName = "Sterling Towers, Marina",
                City = "Lagos",
                Country="Nigeria"
            }) ;

            modelBuilder.Entity<BranchRequest>().HasKey(x => x.Id);

            modelBuilder.Entity<Core.Entities.Branch>()
                          .HasMany(e => e.BranchRequests)
                          .WithMany(e => e.Branches)
                          .UsingEntity<BranchBranchRequest>();


            modelBuilder.Entity<BranchMember>()
            .Property(c => c.User_Id).HasMaxLength(100);
            modelBuilder.Entity<BranchMember>()
               .Property(c => c.User_Name).HasMaxLength(255);

        }
    }
}
