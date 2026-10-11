using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurveyDataEntry.Application;
using SurveyDataEntry.Infrastructure.Identity;

namespace SurveyDataEntry.Infrastructure;

public sealed class AssignmentRow
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid SurveyId { get; set; }
    public string OperatorId { get; set; } = "";
    public bool Blocked { get; set; }
}

public sealed class OperatorStore(ApplicationDbContext db, ILookupNormalizer normalizer) : IOperatorStore
{
    private async Task EnabledAsync(Guid id, CancellationToken ct)
    {
        if (!await db.Users.AnyAsync(u => u.Id == id.ToString() && u.EmailConfirmed && !u.IsBlocked, ct))
            throw new UnauthorizedAccessException("ACCOUNT_ACCESS_DENIED");
    }
    private async Task<SurveyRow> OwnerAsync(Guid owner, Guid surveyId, CancellationToken ct)
    {
        await EnabledAsync(owner, ct);
        return await db.Surveys.AsNoTracking().SingleOrDefaultAsync(s => s.Id == surveyId && s.OwnerId == owner, ct)
            ?? throw new UnauthorizedAccessException("SURVEY_ACCESS_DENIED");
    }
    public async Task<IReadOnlyList<OperatorAssignment>> ListAsync(Guid ownerId, Guid surveyId, CancellationToken ct)
    {
        await OwnerAsync(ownerId, surveyId, ct);
        return await (from assignment in db.Assignments.AsNoTracking()
            join user in db.Users on assignment.OperatorId equals user.Id
            where assignment.SurveyId == surveyId
            orderby user.Email
            select new OperatorAssignment(assignment.Id, user.Email!, assignment.Blocked)).ToListAsync(ct);
    }
    public async Task AssignAsync(Guid ownerId, Guid surveyId, string email, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var survey = await OwnerAsync(ownerId, surveyId, ct);
        if (survey.Closed) throw new InvalidOperationException("SURVEY_CLOSED");
        var normalized = normalizer.NormalizeEmail(email);
        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.NormalizedEmail == normalized && u.EmailConfirmed && !u.IsBlocked, ct)
            ?? throw new ArgumentException("OPERATOR_NOT_AVAILABLE");
        if (user.Id == ownerId.ToString()) throw new ArgumentException("OWNER_ALREADY_AUTHORIZED");
        if (!await db.Assignments.AnyAsync(a => a.SurveyId == surveyId && a.OperatorId == user.Id, ct))
        {
            db.Assignments.Add(new AssignmentRow { SurveyId = surveyId, OperatorId = user.Id });
            await db.SaveChangesAsync(ct);
        }
        await transaction.CommitAsync(ct);
    }
    public async Task RevokeAsync(Guid ownerId, Guid surveyId, Guid assignmentId, CancellationToken ct)
    {
        await OwnerAsync(ownerId, surveyId, ct);
        await db.Assignments.Where(a => a.Id == assignmentId && a.SurveyId == surveyId).ExecuteDeleteAsync(ct);
    }
    public async Task SetBlockedAsync(Guid ownerId, Guid surveyId, Guid assignmentId, bool blocked, CancellationToken ct)
    {
        await OwnerAsync(ownerId, surveyId, ct);
        await db.Assignments.Where(a => a.Id == assignmentId && a.SurveyId == surveyId)
            .ExecuteUpdateAsync(update => update.SetProperty(a => a.Blocked, blocked), ct);
    }
    public async Task<IReadOnlyList<AssignedSurvey>> AssignedAsync(Guid accountId, CancellationToken ct)
    {
        await EnabledAsync(accountId, ct);
        return await (from assignment in db.Assignments.AsNoTracking()
            join survey in db.Surveys on assignment.SurveyId equals survey.Id
            where assignment.OperatorId == accountId.ToString() && !assignment.Blocked && !survey.Closed
            orderby survey.Title
            select new AssignedSurvey(survey.Id, survey.Title)).ToListAsync(ct);
    }
}
