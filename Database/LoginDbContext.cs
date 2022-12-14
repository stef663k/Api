using LoginTreasureApi.Models;

namespace LoginTreasureApi.Database;

public partial class LoginDbContext : DbContext
{
    public LoginDbContext()
    {

    }
    public LoginDbContext(DbContextOptions<LoginDbContext> options) : base(options)
    {

    }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(e => e.ExpiryDate).HasColumnType("smalldatetime");

            entity.Property(e => e.TokenHash)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.TokenSalt)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(e => e.Ts)
                .HasColumnType("smalldatetime")
                .HasColumnName("TS");

            entity.HasOne(e => e.User)
                .WithMany(r => r.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshToken_User");

            entity.ToTable("RefreshToken");
        });
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.PasswordSalt)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Ts)
                .HasColumnType("smalldatetime")
                .HasColumnName("TS");

            entity.ToTable("User");

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    
}
