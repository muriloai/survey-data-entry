using SurveyDataEntry.Domain;

namespace SurveyDataEntry.Application;

public interface IFormStore
{
    Task<Guid> SaveAsync(Guid accountId, Guid surveyId, Guid submissionId, IReadOnlyList<Answer> answers, CancellationToken ct);
    Task<int> CountAsync(Guid accountId, Guid surveyId, CancellationToken ct);
}

public sealed class FormEntry(ICurrentAccount account, IFormStore store)
{
    private Guid AccountId => Guid.TryParse(account.Id, out var id) && id != Guid.Empty
        ? id : throw new UnauthorizedAccessException("ACCOUNT_ACCESS_DENIED");

    public Task<Guid> SaveAsync(Guid surveyId, Guid submissionId, IReadOnlyList<Answer> answers, bool reviewed, CancellationToken ct = default)
    {
        if (!reviewed) throw new ArgumentException("REVIEW_REQUIRED");
        if (submissionId == Guid.Empty) throw new ArgumentException("SUBMISSION_ID_REQUIRED");
        return store.SaveAsync(AccountId, surveyId, submissionId, answers, ct);
    }

    public Task<int> CountAsync(Guid surveyId, CancellationToken ct = default) => store.CountAsync(AccountId, surveyId, ct);
}
