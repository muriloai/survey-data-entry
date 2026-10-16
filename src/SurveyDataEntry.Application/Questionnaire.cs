using SurveyDataEntry.Domain;

namespace SurveyDataEntry.Application;

public interface IQuestionnaireStore
{
    Task<Survey> LoadAsync(Guid ownerId, Guid surveyId, CancellationToken ct);
    Task ChangeAsync(Guid ownerId, Guid surveyId, Action<Survey> change, CancellationToken ct);
}

public sealed class Questionnaire(ICurrentAccount account, ICatalogStore accounts, IQuestionnaireStore store)
{
    private async Task<Guid> OwnerAsync(CancellationToken ct)
    {
        if (!Guid.TryParse(account.Id, out var id) || !await accounts.IsAccountEnabledAsync(account.Id!, ct))
            throw new UnauthorizedAccessException("ACCOUNT_ACCESS_DENIED");
        return id;
    }

    public async Task<Survey> GetAsync(Guid surveyId, CancellationToken ct = default) =>
        await store.LoadAsync(await OwnerAsync(ct), surveyId, ct);

    public async Task AddQuestionAsync(Guid surveyId, string title, QuestionType type, bool required,
        int? maxLength, IEnumerable<string> options, CancellationToken ct = default)
    {
        var owner = await OwnerAsync(ct);
        if (string.IsNullOrWhiteSpace(title) || title.Length > 2000) throw new ArgumentException("QUESTION_TITLE_INVALID");
        var values = options.ToArray();
        if (values.Length > 100 || values.Any(v => v.Length > 500)) throw new ArgumentException("OPTIONS_INVALID");
        var question = new Question(title, type, required, maxLength, values);
        await store.ChangeAsync(owner, surveyId, survey => survey.AddQuestion(question), ct);
    }

    public async Task RemoveQuestionAsync(Guid surveyId, Guid questionId, CancellationToken ct = default) =>
        await store.ChangeAsync(await OwnerAsync(ct), surveyId, survey => survey.RemoveQuestion(questionId), ct);

    public async Task FreezeAsync(Guid surveyId, CancellationToken ct = default)
    {
        var owner = await OwnerAsync(ct);
        await store.ChangeAsync(owner, surveyId, survey =>
        {
            survey.LockStructure();
        }, ct);
    }

    public async Task CloseAsync(Guid surveyId, bool confirmed, CancellationToken ct = default)
    {
        if (!confirmed) throw new ArgumentException("CLOSURE_CONFIRMATION_REQUIRED");
        await store.ChangeAsync(await OwnerAsync(ct), surveyId, survey => survey.Close(), ct);
    }
}
