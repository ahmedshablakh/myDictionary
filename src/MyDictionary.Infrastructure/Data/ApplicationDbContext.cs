using Microsoft.EntityFrameworkCore;
using MyDictionary.Core.Entities;

namespace MyDictionary.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Word> Words { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<TestResult> TestResults { get; set; }
    public DbSet<DailyChallenge> DailyChallenges { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Word entity configuration
        modelBuilder.Entity<Word>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WordText).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Translation).IsRequired().HasMaxLength(200);
            entity.Property(e => e.LanguageFrom).HasMaxLength(10);
            entity.Property(e => e.LanguageTo).HasMaxLength(10);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.ExampleSentence).HasMaxLength(500);
            entity.Property(e => e.ExampleTranslation).HasMaxLength(500);
            
            // Ignore computed properties
            entity.Ignore(e => e.SourceLanguage);
            entity.Ignore(e => e.TargetLanguage);
            entity.Ignore(e => e.AccuracyRate);
            
            entity.HasIndex(e => e.WordText);
            entity.HasIndex(e => e.NextReviewDate);
            entity.HasIndex(e => new { e.LanguageFrom, e.LanguageTo });
        });

        // UserSettings entity configuration
        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DefaultLanguageFrom).HasMaxLength(10);
            entity.Property(e => e.DefaultLanguageTo).HasMaxLength(10);
        });

        // TestResult entity configuration
        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TestDate);
        });

        // DailyChallenge entity configuration
        modelBuilder.Entity<DailyChallenge>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ChallengeName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Date);
        });

        // Seed default settings
        modelBuilder.Entity<UserSettings>().HasData(
            new UserSettings
            {
                Id = 1,
                DefaultLanguageFrom = "en",
                DefaultLanguageTo = "ar",
                DarkMode = true,
                DailyGoal = 20,
                EnableNotifications = true,
                CardsPerSession = 10
            }
        );
    }
}
