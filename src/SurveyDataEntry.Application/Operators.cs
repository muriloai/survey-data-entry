namespace SurveyDataEntry.Application;

public sealed record OperatorAssignment(Guid Id, string Email, bool Blocked);
public sealed record AssignedSurvey(Guid Id, string Title);

public interface IOperatorStore
{
    Task<IReadOnlyList<OperatorAssignment>> ListAsync(Guid ownerId, Guid surveyId, CancellationToken ct);
    Task AssignAsync(Guid ownerId, Guid surveyId, string email, CancellationToken ct);
    Task RevokeAsync(Guid ownerId, Guid surveyId, Guid assignmentId, CancellationToken ct);
    Task SetBlockedAsync(Guid ownerId, Guid surveyId, Guid assignmentId, bool blocked, CancellationToken ct);
    Task<IReadOnlyList<AssignedSurvey>> AssignedAsync(Guid accountId, CancellationToken ct);
}

public sealed class Operators(ICurrentAccount account, IOperatorStore store)
{
    private Guid Account => Guid.TryParse(account.Id, out var id) && id != Guid.Empty ? id : throw new UnauthorizedAccessException("ACCOUNT_ACCESS_DENIED");
    public Task<IReadOnlyList<OperatorAssignment>> ListAsync(Guid surveyId, CancellationToken ct = default) => store.ListAsync(Account, surveyId, ct);
    public Task AssignAsync(Guid surveyId, string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 254) throw new ArgumentException("OPERATOR_EMAIL_INVALID");
        return store.AssignAsync(Account, surveyId, email.Trim(), ct);
    }
    public Task RevokeAsync(Guid surveyId, Guid assignmentId, CancellationToken ct = default) => store.RevokeAsync(Account, surveyId, assignmentId, ct);
    public Task SetBlockedAsync(Guid surveyId, Guid assignmentId, bool blocked, CancellationToken ct = default) => store.SetBlockedAsync(Account, surveyId, assignmentId, blocked, ct);
    public Task<IReadOnlyList<AssignedSurvey>> AssignedAsync(CancellationToken ct = default) => store.AssignedAsync(Account, ct);
}
