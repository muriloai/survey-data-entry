using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SurveyDataEntry.Application;
using SurveyDataEntry.Domain;
using SurveyDataEntry.Infrastructure.Identity;

namespace SurveyDataEntry.Infrastructure;

public sealed class QuestionnaireStore(ApplicationDbContext db) : IQuestionnaireStore
{
    private sealed record SavedQuestion(Guid Id, string Title, QuestionType Type, bool Required, int? MaxLength, string[] Options);

    private static Survey Restore(SurveyRow row)
    {
        var saved = JsonSerializer.Deserialize<SavedQuestion[]>(row.QuestionsJson)
            ?? throw new InvalidOperationException("QUESTIONNAIRE_INVALID");
        return Survey.Restore(row.Id, row.OwnerId, row.ClientId, row.Title, row.Mode,
            saved.Select(q => Question.Restore(q.Id, q.Title, q.Type, q.Required, q.MaxLength, q.Options)),
            row.StructureLocked, row.Closed);
    }

    public async Task<Survey> LoadAsync(Guid ownerId, Guid surveyId, CancellationToken ct)
    {
        var row = await db.Surveys.AsNoTracking().SingleOrDefaultAsync(s => s.Id == surveyId && s.OwnerId == ownerId, ct)
            ?? throw new UnauthorizedAccessException("SURVEY_ACCESS_DENIED");
        return Restore(row);
    }

    public async Task ChangeAsync(Guid ownerId, Guid surveyId, Action<Survey> change, CancellationToken ct)
    {
        var row = await db.Surveys.SingleOrDefaultAsync(s => s.Id == surveyId && s.OwnerId == ownerId, ct)
            ?? throw new UnauthorizedAccessException("SURVEY_ACCESS_DENIED");
        await db.Entry(row).ReloadAsync(ct);
        var survey = Restore(row);
        change(survey);
        row.QuestionsJson = JsonSerializer.Serialize(survey.Questions.Select(q =>
            new SavedQuestion(q.Id, q.Title, q.Type, q.Required, q.MaxLength, q.Options.ToArray())));
        row.StructureLocked = survey.StructureLocked;
        row.Closed = survey.Closed;
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException)
        {
            db.Entry(row).State = EntityState.Detached;
            throw new InvalidOperationException("SURVEY_CONCURRENT_CHANGE");
        }
    }
}
