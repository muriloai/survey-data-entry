using System.Security.Claims;
using SurveyDataEntry.Application;

namespace SurveyDataEntry.Web;

public sealed class CurrentAccount(IHttpContextAccessor accessor) : ICurrentAccount
{
    public string? Id => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
}
