using Microsoft.AspNetCore.Identity;

namespace SurveyDataEntry.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public bool IsBlocked { get; set; }
    public ApplicationUser() => Id = Guid.CreateVersion7().ToString();
}

