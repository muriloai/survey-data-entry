using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SurveyDataEntry.Application;
using SurveyDataEntry.Domain;
using SurveyDataEntry.Infrastructure.Identity;

namespace SurveyDataEntry.Infrastructure;

public sealed class FormRow
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    public Guid OperatorId { get; set; }
    public Guid SubmissionId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public string AnswersJson { get; set; } = "[]";
    public string Fingerprint { get; set; } = "";
}

public sealed class FormStore(ApplicationDbContext db) : IFormStore
{
    private async Task<SurveyRow> AuthorizeAsync(Guid accountId, Guid surveyId, CancellationToken ct)
    {
        if (!await db.Users.AnyAsync(u => u.Id == accountId.ToString() && u.EmailConfirmed && !u.IsBlocked, ct))
            throw new UnauthorizedAccessException("ACCOUNT_ACCESS_DENIED");
        return await db.Surveys.SingleOrDefaultAsync(s => s.Id == surveyId && s.OwnerId == accountId, ct)
            ?? throw new UnauthorizedAccessException("SURVEY_ACCESS_DENIED");
    }

    public async Task<Guid> SaveAsync(Guid accountId, Guid surveyId, Guid submissionId, IReadOnlyList<Answer> answers, CancellationToken ct)
    {
        if (answers is null || answers.Count > 1000 || answers.Any(a => a is null || a.Values is null || a.Values.Count > 100 || a.Values.Any(v => v is null || v.Length > 100000)))
            throw new ArgumentException("ANSWERS_INVALID");
        if (answers.Sum(a => a.Values.Sum(v => (long)v.Length)) > 1000000)
            throw new ArgumentException("ANSWERS_TOO_LARGE");
        var json = JsonSerializer.Serialize(answers.OrderBy(a => a.QuestionId));
        var fingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json)));
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var row = await AuthorizeAsync(accountId, surveyId, ct);
        await db.Entry(row).ReloadAsync(ct);
        if (row.Closed) throw new InvalidOperationException("SURVEY_CLOSED");
        var previous = await db.Forms.AsNoTracking().SingleOrDefaultAsync(f => f.SurveyId == surveyId && f.SubmissionId == submissionId, ct);
        if (previous is not null)
        {
            if (previous.OperatorId != accountId || previous.Fingerprint != fingerprint)
                throw new InvalidOperationException("SUBMISSION_CONFLICT");
            await transaction.CommitAsync(ct);
            return previous.Id;
        }
        var survey = await new QuestionnaireStore(db).LoadAsync(accountId, surveyId, ct);
        var form = new CompletedForm(survey, accountId, answers);
        row.StructureLocked = survey.StructureLocked;
        db.Forms.Add(new FormRow { Id = form.Id, SurveyId = surveyId, OperatorId = accountId, SubmissionId = submissionId,
            CreatedAtUtc = form.CreatedAtUtc, AnswersJson = json, Fingerprint = fingerprint });
        try
        {
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return form.Id;
        }
        catch
        {
            db.ChangeTracker.Clear();
            throw;
        }
    }

    public async Task<int> CountAsync(Guid accountId, Guid surveyId, CancellationToken ct)
    {
        await AuthorizeAsync(accountId, surveyId, ct);
        return await db.Forms.CountAsync(f => f.SurveyId == surveyId, ct);
    }
}
