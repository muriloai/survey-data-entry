using Microsoft.AspNetCore.Identity;

namespace SurveyDataEntry.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser() => Id = Guid.CreateVersion7().ToString();
}

