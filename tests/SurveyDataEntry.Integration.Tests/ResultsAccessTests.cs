using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurveyDataEntry.Application;
using SurveyDataEntry.Domain;
using SurveyDataEntry.Infrastructure;
using SurveyDataEntry.Infrastructure.Identity;
using Xunit;

namespace SurveyDataEntry.Integration.Tests;

public sealed class ResultsAccessTests
{
    private sealed record Account(string Id) : ICurrentAccount;

    [Fact]
    public async Task ResultsArePaginatedAndPrivateAndClosureStopsCollection()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer($"Server=(localdb)\\MSSQLLocalDB;Database=SurveyDataEntry_Test_{Guid.NewGuid():N};Trusted_Connection=True;TrustServerCertificate=True"));
        services.AddIdentityCore<ApplicationUser>(o => o.Stores.SchemaVersion = IdentitySchemaVersions.Version3).AddEntityFrameworkStores<ApplicationDbContext>();
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try
        {
            await db.Database.MigrateAsync();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var owner = new ApplicationUser { UserName = "owner@example.test", Email = "owner@example.test", EmailConfirmed = true };
            var operatorUser = new ApplicationUser { UserName = "operator@example.test", Email = "operator@example.test", EmailConfirmed = true };
            Assert.True((await users.CreateAsync(owner)).Succeeded);
            Assert.True((await users.CreateAsync(operatorUser)).Succeeded);
            var catalogStore = new CatalogStore(db);
            var questionnaireStore = new QuestionnaireStore(db);
            var catalog = new SurveyCatalog(new Account(owner.Id), catalogStore);
            var client = await catalog.CreateClientAsync("Client");
            var surveyId = await catalog.CreateSurveyAsync(client, "Results", SurveyMode.Paper);
            var editor = new Questionnaire(new Account(owner.Id), catalogStore, questionnaireStore);
            await editor.AddQuestionAsync(surveyId, "Choice", QuestionType.SingleChoice, true, null, ["Yes", "No"]);
            await editor.AddQuestionAsync(surveyId, "Optional", QuestionType.MultipleChoice, false, null, ["A", "B"]);
            var questions = (await editor.GetAsync(surveyId)).Questions;
            var results = new SurveyResults(new Account(owner.Id), catalogStore, questionnaireStore, new ResultsStore(db));
            Assert.Equal(0, (await results.GetAsync(surveyId)).Total);
            var entry = new FormEntry(new Account(owner.Id), new FormStore(db));
            for (var i = 0; i < 27; i++)
            {
                var answers = new List<Answer> { new(questions[0].Id, [i % 2 == 0 ? "Yes" : "No"]) };
                if (i % 3 == 0) answers.Add(new Answer(questions[1].Id, ["A", "B"]));
                await entry.SaveAsync(surveyId, Guid.CreateVersion7(), answers, true);
            }
            var first = await results.GetAsync(surveyId);
            var second = await results.GetAsync(surveyId, 2);
            Assert.Equal(27, first.Total);
            Assert.Equal(25, first.Entries.Count);
            Assert.Equal(2, second.Entries.Count);
            Assert.Empty(first.Entries.Select(e => e.Id).Intersect(second.Entries.Select(e => e.Id)));
            Assert.Equal(14, first.Questions[0].Options["Yes"]);
            Assert.Equal(13, first.Questions[0].Options["No"]);
            Assert.Equal(9, first.Questions[1].Answered);
            Assert.Equal(9, first.Questions[1].Options["A"]);
            Assert.Equal(9, first.Questions[1].Options["B"]);
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => results.GetAsync(surveyId, 0));
            var operators = new Operators(new Account(owner.Id), new OperatorStore(db, new UpperInvariantLookupNormalizer()));
            await operators.AssignAsync(surveyId, operatorUser.Email!);
            var operatorResults = new SurveyResults(new Account(operatorUser.Id), catalogStore, questionnaireStore, new ResultsStore(db));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => operatorResults.GetAsync(surveyId));
            await Assert.ThrowsAsync<ArgumentException>(() => editor.CloseAsync(surveyId, false));
            Assert.False((await editor.GetAsync(surveyId)).Closed);
            await editor.CloseAsync(surveyId, true);
            Assert.True((await results.GetAsync(surveyId)).Closed);
            Assert.Equal(27, (await results.GetAsync(surveyId)).Total);
            await Assert.ThrowsAsync<InvalidOperationException>(() => entry.SaveAsync(surveyId, Guid.CreateVersion7(), [new(questions[0].Id, ["Yes"])], true));
            var operatorEntry = new FormEntry(new Account(operatorUser.Id), new FormStore(db));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => operatorEntry.GetAsync(surveyId));
        }
        finally { await db.Database.EnsureDeletedAsync(); }
    }
}
