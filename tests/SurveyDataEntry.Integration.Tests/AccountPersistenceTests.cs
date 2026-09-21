using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurveyDataEntry.Infrastructure.Identity;
using Xunit;

public sealed class AccountPersistenceTests
{
    [Fact]
    public async Task MigrationAndPasswordResetPersistAndRejectReplayedToken()
    {
        var database = "SurveyDataEntry_Test_" + Guid.NewGuid().ToString("N");
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDataProtection();
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer($"Server=(localdb)\\MSSQLLocalDB;Database={database};Trusted_Connection=True;TrustServerCertificate=True"));
        services.AddIdentityCore<ApplicationUser>(options => options.Stores.SchemaVersion = IdentitySchemaVersions.Version3)
            .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            await db.Database.MigrateAsync();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = new ApplicationUser { UserName = "operator@example.test", Email = "operator@example.test" };
            Assert.True((await users.CreateAsync(user, "Initial-Password123!")).Succeeded);
            var confirmation = await users.GenerateEmailConfirmationTokenAsync(user);
            Assert.True((await users.ConfirmEmailAsync(user, confirmation)).Succeeded);
            var reset = await users.GeneratePasswordResetTokenAsync(user);
            Assert.True((await users.ResetPasswordAsync(user, reset, "New-Password456!")).Succeeded);
            Assert.False((await users.ResetPasswordAsync(user, reset, "Replay-Password789!")).Succeeded);
            db.ChangeTracker.Clear();
            var persisted = await users.FindByEmailAsync(user.Email);
            Assert.NotNull(persisted);
            Assert.True(persisted.EmailConfirmed);
            Assert.True(await users.CheckPasswordAsync(persisted, "New-Password456!"));
            Assert.False(await users.CheckPasswordAsync(persisted, "Initial-Password123!"));
        }
        finally { await db.Database.EnsureDeletedAsync(); }
    }
}
