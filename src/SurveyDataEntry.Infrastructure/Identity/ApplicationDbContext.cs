using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SurveyDataEntry.Infrastructure.Identity;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<ClientRow> Clients => Set<ClientRow>();
    public DbSet<SurveyRow> Surveys => Set<SurveyRow>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ClientRow>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(c => c.Id);
            entity.HasAlternateKey(c => new { c.Id, c.OwnerId });
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.HasIndex(c => c.OwnerId);
        });
        builder.Entity<SurveyRow>(entity =>
        {
            entity.ToTable("Surveys");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.QuestionsJson).IsRequired().HasDefaultValue("[]");
            entity.Property(s => s.Version).IsRowVersion();
            entity.Property(s => s.Title).HasMaxLength(200).IsRequired();
            entity.HasIndex(s => s.OwnerId);
            entity.HasOne<ClientRow>().WithMany().HasForeignKey(s => new { s.ClientId, s.OwnerId })
                .HasPrincipalKey(c => new { c.Id, c.OwnerId }).OnDelete(DeleteBehavior.Restrict);
        });
    }
}

