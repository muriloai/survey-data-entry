using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SurveyDataEntry.Application;
using SurveyDataEntry.Domain;
using SurveyDataEntry.Infrastructure.Identity;

namespace SurveyDataEntry.Infrastructure;

public sealed class ResultsStore(ApplicationDbContext db) : IResultsStore
{
    public async IAsyncEnumerable<ResultEntry> ReadAsync(Guid ownerId, Guid surveyId, [EnumeratorCancellation] CancellationToken ct)
    {
        if (!await db.Users.AnyAsync(u => u.Id == ownerId.ToString() && u.EmailConfirmed && !u.IsBlocked, ct) ||
            !await db.Surveys.AnyAsync(s => s.Id == surveyId && s.OwnerId == ownerId, ct))
            throw new UnauthorizedAccessException("SURVEY_ACCESS_DENIED");
        var rows = db.Forms.AsNoTracking().Where(f => f.SurveyId == surveyId)
            .OrderByDescending(f => f.CreatedAtUtc).ThenByDescending(f => f.Id)
            .Select(f => new { f.Id, f.CreatedAtUtc, f.AnswersJson }).AsAsyncEnumerable();
        await foreach (var row in rows.WithCancellation(ct))
            yield return new ResultEntry(row.Id, row.CreatedAtUtc,
                JsonSerializer.Deserialize<Answer[]>(row.AnswersJson) ?? throw new InvalidOperationException("ANSWERS_INVALID"));
    }
}
