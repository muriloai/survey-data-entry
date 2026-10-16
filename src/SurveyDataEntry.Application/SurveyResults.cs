using SurveyDataEntry.Domain;

namespace SurveyDataEntry.Application;

public sealed record ResultEntry(Guid Id, DateTimeOffset CreatedAtUtc, IReadOnlyList<Answer> Answers);
public sealed record QuestionResult(Guid Id, string Title, QuestionType Type, long Answered, IReadOnlyDictionary<string, long> Options);
public sealed record ResultReport(string Title, bool Closed, long Total, int Page, int PageSize,
    IReadOnlyList<QuestionResult> Questions, IReadOnlyList<ResultEntry> Entries);

public interface IResultsStore
{
    IAsyncEnumerable<ResultEntry> ReadAsync(Guid ownerId, Guid surveyId, CancellationToken ct);
}

public sealed class SurveyResults(ICurrentAccount account, ICatalogStore accounts, IQuestionnaireStore questionnaires, IResultsStore store)
{
    public async Task<ResultReport> GetAsync(Guid surveyId, int page = 1, CancellationToken ct = default)
    {
        if (page is < 1 or > 100000) throw new ArgumentOutOfRangeException(nameof(page));
        if (!Guid.TryParse(account.Id, out var owner) || !await accounts.IsAccountEnabledAsync(account.Id!, ct))
            throw new UnauthorizedAccessException("ACCOUNT_ACCESS_DENIED");
        var survey = await questionnaires.LoadAsync(owner, surveyId, ct);
        const int pageSize = 25;
        var answered = survey.Questions.ToDictionary(q => q.Id, _ => 0L);
        var options = survey.Questions.ToDictionary(q => q.Id, q => q.Options.ToDictionary(o => o, _ => 0L, StringComparer.Ordinal));
        var entries = new List<ResultEntry>();
        long total = 0;
        var offset = (long)(page - 1) * pageSize;
        await foreach (var entry in store.ReadAsync(owner, surveyId, ct))
        {
            if (total >= offset && entries.Count < pageSize) entries.Add(entry);
            total++;
            foreach (var answer in entry.Answers)
            {
                if (!answered.ContainsKey(answer.QuestionId) || answer.Values.Count == 0) continue;
                answered[answer.QuestionId]++;
                foreach (var value in answer.Values)
                    if (options[answer.QuestionId].ContainsKey(value)) options[answer.QuestionId][value]++;
            }
        }
        return new ResultReport(survey.Title, survey.Closed, total, page, pageSize,
            survey.Questions.Select(q => new QuestionResult(q.Id, q.Title, q.Type, answered[q.Id], options[q.Id])).ToArray(), entries);
    }
}
