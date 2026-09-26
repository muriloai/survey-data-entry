using SurveyDataEntry.Contracts;
using SurveyDataEntry.Domain;

namespace SurveyDataEntry.Application;

public interface ICurrentAccount
{
    string? Id { get; }
}

public interface ICatalogStore
{
    Task<bool> IsAccountEnabledAsync(string accountId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ClientSummary>> ListClientsAsync(Guid ownerId, CancellationToken cancellationToken);
    Task AddClientAsync(Guid id, Guid ownerId, string name, CancellationToken cancellationToken);
    Task<bool> OwnsClientAsync(Guid ownerId, Guid clientId, CancellationToken cancellationToken);
    Task<IReadOnlyList<SurveySummary>> ListSurveysAsync(Guid ownerId, CancellationToken cancellationToken);
    Task AddSurveyAsync(Survey survey, CancellationToken cancellationToken);
}

public sealed class SurveyCatalog(ICurrentAccount currentAccount, ICatalogStore store)
{
    private async Task<Guid> OwnerAsync(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentAccount.Id, out var id) ||
            !await store.IsAccountEnabledAsync(currentAccount.Id!, cancellationToken))
            throw new UnauthorizedAccessException("ACCOUNT_ACCESS_DENIED");
        return id;
    }

    public async Task<IReadOnlyList<ClientSummary>> ListClientsAsync(CancellationToken cancellationToken = default) =>
        await store.ListClientsAsync(await OwnerAsync(cancellationToken), cancellationToken);

    public async Task<Guid> CreateClientAsync(string name, CancellationToken cancellationToken = default)
    {
        var owner = await OwnerAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200)
            throw new ArgumentException("CLIENT_NAME_INVALID", nameof(name));
        var id = Guid.CreateVersion7();
        await store.AddClientAsync(id, owner, name.Trim(), cancellationToken);
        return id;
    }

    public async Task<IReadOnlyList<SurveySummary>> ListSurveysAsync(CancellationToken cancellationToken = default) =>
        await store.ListSurveysAsync(await OwnerAsync(cancellationToken), cancellationToken);

    public async Task<Guid> CreateSurveyAsync(Guid clientId, string title, SurveyMode mode, CancellationToken cancellationToken = default)
    {
        var owner = await OwnerAsync(cancellationToken);
        if (!await store.OwnsClientAsync(owner, clientId, cancellationToken))
            throw new UnauthorizedAccessException("CLIENT_ACCESS_DENIED");
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length > 200)
            throw new ArgumentException("SURVEY_TITLE_INVALID", nameof(title));
        var survey = new Survey(owner, clientId, title, mode);
        await store.AddSurveyAsync(survey, cancellationToken);
        return survey.Id;
    }
}
