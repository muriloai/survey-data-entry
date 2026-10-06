using System.Globalization;

namespace SurveyDataEntry.Domain;

public sealed record Answer(Guid QuestionId, IReadOnlyList<string> Values);

public sealed class CompletedForm
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public Guid SurveyId { get; }
    public Guid OperatorId { get; }
    public DateTimeOffset CreatedAtUtc { get; } = DateTimeOffset.UtcNow;
    public IReadOnlyList<Answer> Answers { get; }

    public CompletedForm(Survey survey, Guid operatorId, IEnumerable<Answer> answers)
    {
        ArgumentNullException.ThrowIfNull(survey);
        ArgumentNullException.ThrowIfNull(answers);
        if (operatorId == Guid.Empty) throw new ArgumentException("Operator is required.", nameof(operatorId));
        if (survey.Closed) throw new InvalidOperationException("Survey is closed.");
        var submitted = answers.ToArray();
        if (submitted.Any(a => a is null || a.Values is null) || submitted.Select(a => a.QuestionId).Distinct().Count() != submitted.Length)
            throw new ArgumentException("Answers must be unique and valid.", nameof(answers));
        if (submitted.Any(a => survey.Questions.All(q => q.Id != a.QuestionId)))
            throw new ArgumentException("Unknown question.", nameof(answers));
        foreach (var question in survey.Questions)
        {
            var values = submitted.FirstOrDefault(a => a.QuestionId == question.Id)?.Values ?? [];
            if (values.Any(string.IsNullOrWhiteSpace) || (question.Required && values.Count == 0))
                throw new ArgumentException("Required answer is missing or empty.", nameof(answers));
            if (question.Type != QuestionType.MultipleChoice && values.Count > 1)
                throw new ArgumentException("Only one value is allowed.", nameof(answers));
            if (values.Distinct(StringComparer.Ordinal).Count() != values.Count)
                throw new ArgumentException("Duplicate values are not allowed.", nameof(answers));
            foreach (var value in values)
            {
                if (question.Type == QuestionType.Text && question.MaxLength is int limit && value.Length > limit)
                    throw new ArgumentException("Text is too long.", nameof(answers));
                if (question.Type == QuestionType.Number && !decimal.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _))
                    throw new ArgumentException("Invalid number.", nameof(answers));
                if (question.Type is QuestionType.SingleChoice or QuestionType.MultipleChoice && !question.Options.Contains(value, StringComparer.Ordinal))
                    throw new ArgumentException("Invalid option.", nameof(answers));
            }
        }
        survey.LockStructure();
        SurveyId = survey.Id; OperatorId = operatorId;
        Answers = Array.AsReadOnly(submitted.Select(a => new Answer(a.QuestionId, Array.AsReadOnly(a.Values.ToArray()))).ToArray());
    }
}
