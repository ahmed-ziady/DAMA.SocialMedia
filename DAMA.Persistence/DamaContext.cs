using System;
using System.Collections.Generic;
using DAMA.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace DAMA.Persistence;

public partial class DamaContext : IdentityDbContext<User, Role, int,
       IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>,
       IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<FlaggedPost> FlaggedPosts { get; set; }

    public DbSet<FriendRequest> FriendRequests { get; set; }

    public DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<ModerationAction> ModerationActions { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostReaction> PostReactions { get; set; }

    public virtual DbSet<PostReport> PostReports { get; set; }

    public virtual DbSet<PostType> PostTypes { get; set; }

    public virtual DbSet<ReportType> ReportTypes { get; set; }

    public new virtual DbSet<Role> Roles { get; set; }

    public new virtual DbSet<User> Users { get; set; }


    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public DamaContext(DbContextOptions<DamaContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=NOTEBOOK\\MSSQLSERVER02;Database=DAMA;Integrated Security=SSPI;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__C3B4DFAA2372FD48");

            entity.ToTable("Comment");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.CommentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_Comment_Post");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_User");
        });

        modelBuilder.Entity<FlaggedPost>(entity =>
        {
            entity.HasKey(e => e.FlagId).HasName("PK__FlaggedP__780D45B3C07A5E19");

            entity.ToTable("FlaggedPost");

            entity.Property(e => e.FlagId).HasColumnName("FlagID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.ReviewStatus).HasMaxLength(50);
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.HasOne(d => d.Post).WithMany(p => p.FlaggedPosts)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_FlaggedPost_Post");

            entity.HasOne(d => d.User).WithMany(p => p.FlaggedPosts)
                .HasForeignKey(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FlaggedPost_User");
        });

        modelBuilder.Entity<Friendship>(entity =>
        {

                 entity.HasOne(f => f.Requester)
                .WithMany()
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

                 entity.HasOne(f => f.Receiver)
                .WithMany()
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasKey(f => f.FriendRequestId);

            entity.HasOne(fr => fr.Sender)
            .WithMany(u => u.FriendRequestsSent)
            .HasForeignKey(fr => fr.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(fr => fr.Receiver)
            .WithMany(u => u.FriendRequestsReceived)
            .HasForeignKey(fr => fr.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        });
           




        modelBuilder.Entity<ModerationAction>(entity =>
        {
            entity.HasKey(e => e.ActionId).HasName("PK__Moderati__FFE3F4B97E41AFDB");

            entity.ToTable("ModerationAction");

            entity.Property(e => e.ActionId).HasColumnName("ActionID");
            entity.Property(e => e.ActionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ActionType).HasMaxLength(50);
            entity.Property(e => e.PostId).HasColumnName("PostID");

            entity.HasOne(d => d.Post).WithMany(p => p.ModerationActions)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_ModerationAction_Post");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32FBCDEB63");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NotificationTypeId).HasColumnName("NotificationTypeID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.HasOne(d => d.NotificationType).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.NotificationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_NotificationType");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasKey(e => e.NotificationTypeId).HasName("PK__Notifica__299002A1BAD69468");

            entity.ToTable("NotificationType");

            entity.HasIndex(e => e.TypeName, "UQ__Notifica__D4E7DFA88561D1C2").IsUnique();

            entity.Property(e => e.NotificationTypeId).HasColumnName("NotificationTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Post__AA126038ABBB63BD");

            entity.ToTable("Post");

            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.PostDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PostTypeId).HasColumnName("PostTypeID");
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.HasOne(d => d.PostType).WithMany(p => p.Posts)
                .HasForeignKey(d => d.PostTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Post_PostType");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Post_User");
        });

        modelBuilder.Entity<PostReaction>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.PostId }).HasName("PK__Post_Rea__8D29EAAF20F3EA9C");

            entity.ToTable("Post_Reaction");

            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.ReactionType).HasMaxLength(50);

            entity.HasOne(d => d.Post).WithMany(p => p.PostReactions)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_Post_Reaction_Post");

            entity.HasOne(d => d.User).WithMany(p => p.PostReactions)
                .HasForeignKey(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Post_Reaction_User");
        });

        modelBuilder.Entity<PostReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__PostRepo__D5BD48E5BB9AEAEE");

            entity.ToTable("PostReport");

            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.ReportStatus).HasMaxLength(50);
            entity.Property(e => e.ReportTypeId).HasColumnName("ReportTypeID");
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.HasOne(d => d.Post).WithMany(p => p.PostReports)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_PostReport_Post");

            entity.HasOne(d => d.ReportType).WithMany(p => p.PostReports)
                .HasForeignKey(d => d.ReportTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostReport_ReportType");

            entity.HasOne(d => d.User).WithMany(p => p.PostReports)
                .HasForeignKey(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostReport_User");
        });

        modelBuilder.Entity<PostType>(entity =>
        {
            entity.HasKey(e => e.PostTypeId).HasName("PK__PostType__AB2126101F76BBC8");

            entity.ToTable("PostType");

            entity.HasIndex(e => e.TypeName, "UQ__PostType__D4E7DFA85D56D9B6").IsUnique();

            entity.Property(e => e.PostTypeId).HasColumnName("PostTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<ReportType>(entity =>
        {
            entity.HasKey(e => e.ReportTypeId).HasName("PK__ReportTy__78CF8C83D28F29DC");

            entity.ToTable("ReportType");

            entity.HasIndex(e => e.ReportTypeName, "UQ__ReportTy__D0F3D65679BF4262").IsUnique();

            entity.Property(e => e.ReportTypeId).HasColumnName("ReportTypeID");
            entity.Property(e => e.ReportTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__8AFACE3AA1298EE2");

            entity.ToTable("Role");

            entity.HasIndex(e => e.Name, "UQ__Role__8A2B616047050847").IsUnique();

            entity.Property(e => e.Id).HasColumnName("RoleID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_User"); // Use 'Id' as PK

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ_User_Email").IsUnique();

            entity.Property(e => e.Id) // Use 'Id' as primary key
                .HasColumnName("Id") // Ensure it maps to 'Id' column
                .ValueGeneratedOnAdd(); // Auto-increment (Identity)

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.ProfileImageUrl).HasMaxLength(500);
            entity.Property(e => e.PortfolioImageUrl).HasMaxLength(500);

            // Update User-Roles Many-to-Many Relationship
            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_User_Role_Role"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("Id") // Change from "Id" to "Id"
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_User_Role_User"),
                    j =>
                    {
                        j.HasKey("Id", "RoleId").HasName("PK_User_Role");
                        j.ToTable("User_Role");
                        j.IndexerProperty<int>("Id").HasColumnName("Id"); // Change "Id" to "Id"
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                    });
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserProfileId).HasName("PK__UserProf__9E267F42AABEA4F2");

            entity.ToTable("UserProfile");

            entity.HasIndex(e => e.Id, "UQ__UserProf__1788CCADABECC2C5").IsUnique();

            entity.Property(e => e.UserProfileId).HasColumnName("UserProfileID");
            entity.Property(e => e.Bio).HasMaxLength(2000);
            entity.Property(e => e.ProfilePictureUrl)
                .HasMaxLength(500)
                .HasColumnName("ProfilePictureURL");
            entity.Property(e => e.Id).HasColumnName("Id");

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.Id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserProfile_User");
        });
        modelBuilder.Entity<User>().Property(u => u.Email).IsRequired();
        modelBuilder.Entity<IdentityUserLogin<int>>()
       .HasKey(l => new { l.LoginProvider, l.ProviderKey });

        modelBuilder.Entity<IdentityUserRole<int>>()
            .HasKey(r => new { r.UserId, r.RoleId });

        modelBuilder.Entity<IdentityUserToken<int>>()
            .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });


        // Ensure IdentityRole has a proper primary key mapping
        modelBuilder.Entity<Role>().HasKey(r => r.Id);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

