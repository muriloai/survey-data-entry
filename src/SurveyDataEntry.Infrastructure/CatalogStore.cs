using Microsoft.EntityFrameworkCore;
using SurveyDataEntry.Application;
using SurveyDataEntry.Contracts;
using SurveyDataEntry.Domain;
using SurveyDataEntry.Infrastructure.Identity;

namespace SurveyDataEntry.Infrastructure;

public sealed class ClientRow
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = "";
}

public sealed class SurveyRow
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public Guid ClientId { get; set; }
    public string Title { get; set; } = "";
    public SurveyMode Mode { get; set; }
    public bool Closed { get; set; }
}

public sealed class CatalogStore(ApplicationDbContext db) : ICatalogStore
{
    public Task<bool> IsAccountEnabledAsync(string accountId, CancellationToken ct) =>
        db.Users.AnyAsync(u => u.Id == accountId && u.EmailConfirmed && !u.IsBlocked, ct);

    public async Task<IReadOnlyList<ClientSummary>> ListClientsAsync(Guid ownerId, CancellationToken ct) =>
        await db.Clients.AsNoTracking().Where(c => c.OwnerId == ownerId).OrderBy(c => c.Name)
            .Select(c => new ClientSummary(c.Id, c.Name)).ToListAsync(ct);

    public async Task AddClientAsync(Guid id, Guid ownerId, string name, CancellationToken ct)
    {
        db.Clients.Add(new ClientRow { Id = id, OwnerId = ownerId, Name = name });
        await db.SaveChangesAsync(ct);
    }

    public Task<bool> OwnsClientAsync(Guid ownerId, Guid clientId, CancellationToken ct) =>
        db.Clients.AnyAsync(c => c.Id == clientId && c.OwnerId == ownerId, ct);

    public async Task<IReadOnlyList<SurveySummary>> ListSurveysAsync(Guid ownerId, CancellationToken ct) =>
        await db.Surveys.AsNoTracking().Where(s => s.OwnerId == ownerId).OrderBy(s => s.Title)
            .Select(s => new SurveySummary(s.Id, s.ClientId, s.Title, s.Mode.ToString(), s.Closed)).ToListAsync(ct);

    public async Task AddSurveyAsync(Survey survey, CancellationToken ct)
    {
        db.Surveys.Add(new SurveyRow { Id = survey.Id, OwnerId = survey.OwnerId, ClientId = survey.ClientId,
            Title = survey.Title, Mode = survey.Mode, Closed = survey.Closed });
        await db.SaveChangesAsync(ct);
    }
}
