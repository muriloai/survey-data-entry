namespace SurveyDataEntry.Domain;

public enum SurveyMode { Paper, Online }
public enum QuestionType { Text, Number, SingleChoice, MultipleChoice }

public sealed class Question
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public string Title { get; }
    public QuestionType Type { get; }
    public bool Required { get; }
    public int? MaxLength { get; }
    public IReadOnlyList<string> Options { get; }

    public Question(string title, QuestionType type, bool required, int? maxLength = null, IEnumerable<string>? options = null)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Question title is required.", nameof(title));
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        if (maxLength is <= 0 || (maxLength is not null && type != QuestionType.Text)) throw new ArgumentException("Invalid text limit.", nameof(maxLength));
        var values = (options ?? []).ToArray();
        if (values.Any(string.IsNullOrWhiteSpace) || values.Distinct(StringComparer.Ordinal).Count() != values.Length)
            throw new ArgumentException("Options must be nonempty and unique.", nameof(options));
        if ((type is QuestionType.SingleChoice or QuestionType.MultipleChoice) != (values.Length > 0))
            throw new ArgumentException("Choice questions require options; other questions cannot have options.", nameof(options));
        Title = title.Trim(); Type = type; Required = required; MaxLength = maxLength;
        Options = Array.AsReadOnly(values);
    }
}

public sealed class Survey
{
    private readonly List<Question> questions = [];
    public Guid Id { get; } = Guid.CreateVersion7();
    public Guid OwnerId { get; }
    public Guid ClientId { get; }
    public string Title { get; }
    public SurveyMode Mode { get; }
    public bool StructureLocked { get; private set; }
    public bool Closed { get; private set; }
    public IReadOnlyList<Question> Questions => questions.AsReadOnly();

    public Survey(Guid ownerId, Guid clientId, string title, SurveyMode mode)
    {
        if (ownerId == Guid.Empty || clientId == Guid.Empty) throw new ArgumentException("Owner and client are required.");
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Survey title is required.", nameof(title));
        if (!Enum.IsDefined(mode)) throw new ArgumentOutOfRangeException(nameof(mode));
        OwnerId = ownerId; ClientId = clientId; Title = title.Trim(); Mode = mode;
    }

    public void AddQuestion(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);
        EnsureEditable();
        questions.Add(question);
    }

    public void RemoveQuestion(Guid questionId)
    {
        EnsureEditable();
        var question = questions.Find(q => q.Id == questionId) ?? throw new ArgumentException("Question not found.", nameof(questionId));
        questions.Remove(question);
    }

    public void LockStructure()
    {
        if (Closed) throw new InvalidOperationException("Survey is closed.");
        if (questions.Count == 0) throw new InvalidOperationException("A survey needs at least one question.");
        StructureLocked = true;
    }

    public void Close() => Closed = true;
    private void EnsureEditable()
    {
        if (StructureLocked || Closed) throw new InvalidOperationException("Survey structure cannot be changed.");
    }
}
