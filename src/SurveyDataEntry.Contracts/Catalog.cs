namespace SurveyDataEntry.Contracts;

public sealed record ClientSummary(Guid Id, string Name);
public sealed record SurveySummary(Guid Id, Guid ClientId, string Title, string Mode, bool Closed);
