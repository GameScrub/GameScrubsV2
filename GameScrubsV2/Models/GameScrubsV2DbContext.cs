using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameScrubsV2.Models;

public partial class GameScrubsV2DbContext : IdentityDbContext<IdentityUser>
{
	public GameScrubsV2DbContext()
	{
	}

	public GameScrubsV2DbContext(DbContextOptions<GameScrubsV2DbContext> options)
		: base(options)
	{
	}

	public virtual DbSet<Bracket> Brackets { get; set; }

	public virtual DbSet<BracketPosition> BracketPositions { get; set; }

	public virtual DbSet<BracketScore> BracketScores { get; set; }

	public virtual DbSet<MigrationHistory> MigrationHistories { get; set; }

	public virtual DbSet<Placement> Placements { get; set; }

	public virtual DbSet<PlayerList> PlayerLists { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Bracket>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK_dbo.Brackets");

			entity.Property(e => e.Id).HasColumnName("ID");
			entity.Property(e => e.CreatedDate).HasColumnType("datetime");
			entity.Property(e => e.IsLocked).HasColumnName("isLocked");
			entity.Property(e => e.StartDate).HasColumnType("datetime");
		});

		modelBuilder.Entity<BracketPosition>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK_dbo.BracketPositions");
			entity.Property(e => e.Id).HasColumnName("ID");
		});

		modelBuilder.Entity<BracketScore>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK_dbo.BracketScores");
			entity.Property(e => e.Id).HasColumnName("ID");
		});

		modelBuilder.Entity<MigrationHistory>(entity =>
		{
			entity.HasKey(e => new { e.MigrationId, e.ContextKey }).HasName("PK_dbo.__MigrationHistory");
			entity.ToTable("__MigrationHistory");
			entity.Property(e => e.MigrationId).HasMaxLength(150);
			entity.Property(e => e.ContextKey).HasMaxLength(300);
			entity.Property(e => e.ProductVersion).HasMaxLength(32);
		});

		modelBuilder.Entity<Placement>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK_dbo.Placements");
			entity.HasIndex(e => e.BracketId, "IX_BracketID");
			entity.Property(e => e.Id).HasColumnName("ID");
			entity.Property(e => e.BracketId).HasColumnName("BracketID");

			entity.HasOne(d => d.Bracket).WithMany(p => p.Placements)
				.HasForeignKey(d => d.BracketId)
				.HasConstraintName("FK_dbo.Placements_dbo.Brackets_BracketID");
		});

		modelBuilder.Entity<PlayerList>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK_dbo.PlayerLists");
			entity.HasIndex(e => e.BracketId, "IX_BracketID");
			entity.Property(e => e.Id).HasColumnName("ID");
			entity.Property(e => e.BracketId).HasColumnName("BracketID");

			entity.HasOne(d => d.Bracket).WithMany(p => p.PlayerLists)
				.HasForeignKey(d => d.BracketId)
				.HasConstraintName("FK_dbo.PlayerLists_dbo.Brackets_BracketID");
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}