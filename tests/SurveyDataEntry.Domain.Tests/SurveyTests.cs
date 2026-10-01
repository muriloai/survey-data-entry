using SurveyDataEntry.Domain;
using Xunit;

public class SurveyTests
{
    [Fact]
    public void RestorePreservesIdentityAndFrozenStructure()
    {
        var id = Guid.CreateVersion7();
        var questionId = Guid.CreateVersion7();
        var question = Question.Restore(questionId, "Name", QuestionType.Text, true, 100, []);
        var survey = Survey.Restore(id, Guid.CreateVersion7(), Guid.CreateVersion7(), "Restored", SurveyMode.Paper, [question], true, false);
        Assert.Equal(id, survey.Id);
        Assert.Equal(questionId, survey.Questions[0].Id);
        Assert.Throws<InvalidOperationException>(() => survey.RemoveQuestion(questionId));
    }

    [Fact]
    public void DuplicateQuestionIdentityIsRejected()
    {
        var survey = NewSurvey();
        var question = new Question("Name", QuestionType.Text, true);
        survey.AddQuestion(question);
        Assert.Throws<ArgumentException>(() => survey.AddQuestion(question));
    }
    private static Survey NewSurvey() => new(Guid.CreateVersion7(), Guid.CreateVersion7(), "Survey", SurveyMode.Paper);

    [Fact]
    public void InvalidSubmissionDoesNotLockStructure()
    {
        var survey = NewSurvey();
        var question = new Question("Choice", QuestionType.SingleChoice, true, options: ["A", "B"]);
        survey.AddQuestion(question);
        Assert.Throws<ArgumentException>(() => new CompletedForm(survey, Guid.CreateVersion7(), [new(question.Id, ["C"])]));
        Assert.False(survey.StructureLocked);
    }

    [Fact]
    public void ValidSubmissionLocksStructureAndCopiesInput()
    {
        var survey = NewSurvey();
        var question = new Question("Choice", QuestionType.SingleChoice, true, options: ["A", "B"]);
        survey.AddQuestion(question);
        var values = new List<string> { "A" };
        var form = new CompletedForm(survey, Guid.CreateVersion7(), [new(question.Id, values)]);
        values[0] = "B";
        Assert.Equal("A", form.Answers[0].Values[0]);
        Assert.Throws<InvalidOperationException>(() => survey.RemoveQuestion(question.Id));
    }

    [Fact]
    public void ClosedSurveyRejectsSubmission()
    {
        var survey = NewSurvey();
        survey.Close();
        Assert.Throws<InvalidOperationException>(() => new CompletedForm(survey, Guid.CreateVersion7(), []));
    }

    [Fact]
    public void RequiredQuestionRejectsMissingAnswer()
    {
        var survey = NewSurvey();
        survey.AddQuestion(new Question("Name", QuestionType.Text, true));
        Assert.Throws<ArgumentException>(() => new CompletedForm(survey, Guid.CreateVersion7(), []));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyOptionIsRejected(string option) => Assert.Throws<ArgumentException>(() => new Question("Choice", QuestionType.SingleChoice, true, options: [option]));
}
