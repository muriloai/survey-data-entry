using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurveyDataEntry.Infrastructure.Identity;
using Xunit;
using SurveyDataEntry.Application;
using SurveyDataEntry.Infrastructure;
using SurveyDataEntry.Domain;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

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
            await db.GetService<IMigrator>().MigrateAsync("AddSurveyCatalog");
            var oldClient = Guid.CreateVersion7();
            var oldSurvey = Guid.CreateVersion7();
            var oldOwner = Guid.CreateVersion7();
            await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Clients (Id, OwnerId, Name) VALUES ({oldClient}, {oldOwner}, N'Existing client')");
            await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO Surveys (Id, OwnerId, ClientId, Title, Mode, Closed) VALUES ({oldSurvey}, {oldOwner}, {oldClient}, N'Existing survey', 0, 0)");
            await db.Database.MigrateAsync();
            Assert.Empty((await new QuestionnaireStore(db).LoadAsync(oldOwner, oldSurvey, default)).Questions);
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
            var first = new SurveyCatalog(new TestAccount(user.Id), new CatalogStore(db));
            var clientId = await first.CreateClientAsync("Client A");
            var surveyId = await first.CreateSurveyAsync(clientId, "Survey A", SurveyMode.Paper);
            var otherUser = new ApplicationUser { UserName = "other@example.test", Email = "other@example.test", EmailConfirmed = true };
            Assert.True((await users.CreateAsync(otherUser, "Other-Password123!")).Succeeded);
            var second = new SurveyCatalog(new TestAccount(otherUser.Id), new CatalogStore(db));
            Assert.Empty(await second.ListClientsAsync());
            Assert.Empty(await second.ListSurveysAsync());
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => second.CreateSurveyAsync(clientId, "Forbidden", SurveyMode.Online));
            Assert.Single(await first.ListSurveysAsync());
            var editor = new Questionnaire(new TestAccount(user.Id), new CatalogStore(db), new QuestionnaireStore(db));
            var otherEditor = new Questionnaire(new TestAccount(otherUser.Id), new CatalogStore(db), new QuestionnaireStore(db));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => otherEditor.GetAsync(surveyId));
            await editor.AddQuestionAsync(surveyId, "Choice", QuestionType.SingleChoice, true, null, ["Yes", "No"]);
            var questionnaire = await editor.GetAsync(surveyId);
            Assert.Single(questionnaire.Questions);
            Assert.Equal(new[] { "Yes", "No" }, questionnaire.Questions[0].Options);
            await editor.RemoveQuestionAsync(surveyId, questionnaire.Questions[0].Id);
            Assert.Empty((await editor.GetAsync(surveyId)).Questions);
            await editor.AddQuestionAsync(surveyId, "Name", QuestionType.Text, true, 100, []);
            await editor.FreezeAsync(surveyId);
            Assert.True((await editor.GetAsync(surveyId)).StructureLocked);
            await Assert.ThrowsAsync<InvalidOperationException>(() => editor.AddQuestionAsync(surveyId, "Late", QuestionType.Text, false, null, []));
            var entry = new FormEntry(new TestAccount(user.Id), new FormStore(db));
            var otherEntry = new FormEntry(new TestAccount(otherUser.Id), new FormStore(db));
            var questionId = (await editor.GetAsync(surveyId)).Questions[0].Id;
            await Assert.ThrowsAsync<ArgumentException>(() => entry.SaveAsync(surveyId, Guid.CreateVersion7(), [new(questionId, [new string('x', 101)])], true));
            Assert.Equal(0, await entry.CountAsync(surveyId));
            var submission = Guid.CreateVersion7();
            var savedForm = await entry.SaveAsync(surveyId, submission, [new(questionId, ["Respondent"])], true);
            Assert.Equal(savedForm, await entry.SaveAsync(surveyId, submission, [new(questionId, ["Respondent"])], true));
            Assert.Equal(1, await entry.CountAsync(surveyId));
            await Assert.ThrowsAsync<InvalidOperationException>(() => entry.SaveAsync(surveyId, submission, [new(questionId, ["Changed"])], true));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => otherEntry.SaveAsync(surveyId, Guid.CreateVersion7(), [new(questionId, ["Intruder"])], true));
            await Assert.ThrowsAsync<ArgumentException>(() => entry.SaveAsync(surveyId, Guid.CreateVersion7(), [new(questionId, ["Unreviewed"])], false));
            Assert.Equal(1, await entry.CountAsync(surveyId));
            var draftId = await first.CreateSurveyAsync(clientId, "Draft collection", SurveyMode.Paper);
            await editor.AddQuestionAsync(draftId, "Answer", QuestionType.Text, true, 50, []);
            var draftQuestion = (await editor.GetAsync(draftId)).Questions[0].Id;
            await entry.SaveAsync(draftId, Guid.CreateVersion7(), [new(draftQuestion, ["First response"])], true);
            Assert.True((await editor.GetAsync(draftId)).StructureLocked);
            var concurrentId = await first.CreateSurveyAsync(clientId, "Concurrent", SurveyMode.Paper);
            await using var concurrentDb = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(db.Database.GetConnectionString()).Options);
            var questionnaireStore = new QuestionnaireStore(db);
            await Assert.ThrowsAsync<InvalidOperationException>(() => questionnaireStore.ChangeAsync(Guid.Parse(user.Id), concurrentId, survey =>
            {
                concurrentDb.Surveys.Where(s => s.Id == concurrentId).ExecuteUpdate(setters => setters.SetProperty(s => s.Closed, true));
                survey.AddQuestion(new Question("Conflicting edit", QuestionType.Text, true));
            }, default));
            var afterConflict = await editor.GetAsync(concurrentId);
            Assert.True(afterConflict.Closed);
            Assert.Empty(afterConflict.Questions);
            await concurrentDb.Surveys.Where(s => s.Id == surveyId).ExecuteUpdateAsync(setters => setters.SetProperty(s => s.Closed, true));
            await Assert.ThrowsAsync<InvalidOperationException>(() => entry.SaveAsync(surveyId, Guid.CreateVersion7(), [new(questionId, ["After closure"])], true));
            db.ChangeTracker.Clear();
            persisted = (await users.FindByEmailAsync(user.Email))!;
            persisted.IsBlocked = true;
            await users.UpdateAsync(persisted);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => first.ListClientsAsync());
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => entry.SaveAsync(surveyId, Guid.CreateVersion7(), [new(questionId, ["Blocked"])], true));
        }
        finally { await db.Database.EnsureDeletedAsync(); }
    }

    private sealed record TestAccount(string Id) : ICurrentAccount;
}
