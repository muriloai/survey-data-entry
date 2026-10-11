using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurveyDataEntry.Application;
using SurveyDataEntry.Domain;
using SurveyDataEntry.Infrastructure;
using SurveyDataEntry.Infrastructure.Identity;
using Xunit;

namespace SurveyDataEntry.Integration.Tests;

public sealed class OperatorAccessTests
{
    private sealed record Account(string Id) : ICurrentAccount;

    [Fact]
    public async Task AssignmentsAreScopedAndRevocationAppliesToExistingSession()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer($"Server=(localdb)\\MSSQLLocalDB;Database=SurveyDataEntry_Test_{Guid.NewGuid():N};Trusted_Connection=True;TrustServerCertificate=True"));
        services.AddIdentityCore<ApplicationUser>(o => o.Stores.SchemaVersion = IdentitySchemaVersions.Version3)
            .AddEntityFrameworkStores<ApplicationDbContext>();
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            await db.Database.MigrateAsync();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            async Task<ApplicationUser> Create(string name)
            {
                var user = new ApplicationUser { UserName = name + "@example.test", Email = name + "@example.test", EmailConfirmed = true };
                Assert.True((await users.CreateAsync(user, "Test-Password123!")).Succeeded);
                return user;
            }
            var owner = await Create("owner");
            var anotherOwner = await Create("other");
            var operatorUser = await Create("operator");
            var catalog = new SurveyCatalog(new Account(owner.Id), new CatalogStore(db));
            var client = await catalog.CreateClientAsync("Client");
            var surveyId = await catalog.CreateSurveyAsync(client, "Assigned survey", SurveyMode.Paper);
            var editor = new Questionnaire(new Account(owner.Id), new CatalogStore(db), new QuestionnaireStore(db));
            await editor.AddQuestionAsync(surveyId, "Answer", QuestionType.Text, true, 100, []);
            var questionId = (await editor.GetAsync(surveyId)).Questions[0].Id;
            var store = new OperatorStore(db, new UpperInvariantLookupNormalizer());
            var manager = new Operators(new Account(owner.Id), store);
            var otherManager = new Operators(new Account(anotherOwner.Id), store);
            var portal = new Operators(new Account(operatorUser.Id), store);
            var entry = new FormEntry(new Account(operatorUser.Id), new FormStore(db));
            Assert.Empty(await portal.AssignedAsync());
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => entry.GetAsync(surveyId));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => otherManager.AssignAsync(surveyId, operatorUser.Email!));
            await manager.AssignAsync(surveyId, operatorUser.Email!.ToUpperInvariant());
            await manager.AssignAsync(surveyId, operatorUser.Email!);
            var assignment = Assert.Single(await manager.ListAsync(surveyId));
            Assert.Single(await portal.AssignedAsync());
            Assert.Equal(surveyId, (await entry.GetAsync(surveyId)).Id);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => new Questionnaire(new Account(operatorUser.Id), new CatalogStore(db), new QuestionnaireStore(db)).GetAsync(surveyId));
            await entry.SaveAsync(surveyId, Guid.CreateVersion7(), [new(questionId, ["Operator answer"])], true);
            Assert.Equal(1, await entry.CountAsync(surveyId));
            var otherCatalog = new SurveyCatalog(new Account(anotherOwner.Id), new CatalogStore(db));
            var otherClient = await otherCatalog.CreateClientAsync("Other client");
            var otherSurvey = await otherCatalog.CreateSurveyAsync(otherClient, "Other assigned survey", SurveyMode.Paper);
            await otherManager.AssignAsync(otherSurvey, operatorUser.Email!);
            Assert.Equal(2, (await portal.AssignedAsync()).Count);
            await manager.SetBlockedAsync(surveyId, assignment.Id, true);
            Assert.Equal(otherSurvey, Assert.Single(await portal.AssignedAsync()).Id);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => entry.GetAsync(surveyId));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => entry.SaveAsync(surveyId, Guid.CreateVersion7(), [new(questionId, ["Blocked"])], true));
            await manager.SetBlockedAsync(surveyId, assignment.Id, false);
            Assert.Equal(2, (await portal.AssignedAsync()).Count);
            await manager.RevokeAsync(surveyId, assignment.Id);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => entry.CountAsync(surveyId));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => entry.SaveAsync(surveyId, Guid.CreateVersion7(), [new(questionId, ["Revoked"])], true));
            Assert.Equal(1, await db.Forms.CountAsync());
            await manager.AssignAsync(surveyId, operatorUser.Email!);
            await db.Surveys.Where(s => s.Id == surveyId).ExecuteUpdateAsync(s => s.SetProperty(row => row.Closed, true));
            Assert.Equal(otherSurvey, Assert.Single(await portal.AssignedAsync()).Id);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => entry.GetAsync(surveyId));
        }
        finally { await db.Database.EnsureDeletedAsync(); }
    }
}
