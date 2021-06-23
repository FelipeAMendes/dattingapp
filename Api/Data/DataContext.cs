using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class DataContext : DbContext
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }

        public DataContext(DbContextOptions options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.LikedUserId });

            modelBuilder.Entity<UserLike>()
                .HasOne(u => u.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(f => f.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserLike>()
                .HasOne(u => u.LikedUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(f => f.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}