using SurveyDataEntry.Domain;
using Xunit;

public class SurveyTests
{
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
