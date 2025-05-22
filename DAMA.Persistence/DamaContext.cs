using DAMA.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAMA.Persistence
{
    public class DamaContext(DbContextOptions<DamaContext> options) : IdentityDbContext<User, Role, int,
        IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
    {


        public DbSet<Friendship> Friendships { get; set; } = null!;
        public DbSet<FriendRequest> FriendRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FriendRequest>(entity =>
            {
                entity.HasKey(fr => fr.FriendRequestId);

                entity.HasOne(fr => fr.Sender)
                .WithMany(u => u.SentFriendRequests)
                    .HasForeignKey(fr => fr.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(fr => fr.Receiver)

                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(fr => fr.ReceiverId)
                 .OnDelete(DeleteBehavior.Restrict);
                entity.Property(fr => fr.RequestDate)

                .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(fr => fr.Status)

                .IsRequired();

                entity.HasIndex(fr => new { fr.SenderId, fr.ReceiverId })
                .IsUnique();

            });
            builder.Entity<Friendship>(entity =>
            {
                entity.HasKey(f => f.FriendshipId);
                entity.HasOne(f => f.Requester)
                .WithMany(u => u.FriendshipsRequester)
                    .HasForeignKey(f => f.RequesterId)
                    .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(f => f.Receiver)
                .WithMany(u => u.FriendshipsReceiver)
                    .HasForeignKey(f => f.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(fs => fs.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(fs => new { fs.RequesterId, fs.ReceiverId })
                    .IsUnique();


            });


            // Optional: Customize table names.
            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        }
    }
}
