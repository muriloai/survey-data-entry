using System.Net;
using Microsoft.AspNetCore.Identity;
using SurveyDataEntry.Infrastructure.Identity;

namespace SurveyDataEntry.Web.Components.Account;

internal sealed class DevelopmentEmailSender(IWebHostEnvironment environment) : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        SaveAsync(email, "Confirm your email", $"Confirm your account: {WebUtility.HtmlDecode(confirmationLink)}");

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SaveAsync(email, "Reset your password", $"Reset your password: {WebUtility.HtmlDecode(resetLink)}");

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SaveAsync(email, "Reset your password", $"Reset code: {resetCode}");

    private async Task SaveAsync(string email, string subject, string body)
    {
        if (!environment.IsDevelopment())
            throw new InvalidOperationException("Configure an email provider before running outside development.");
        var directory = Path.Combine(environment.ContentRootPath, "App_Data", "mail");
        Directory.CreateDirectory(directory);
        await File.WriteAllTextAsync(Path.Combine(directory, $"{Guid.CreateVersion7():N}.txt"), $"To: {email}\nSubject: {subject}\n\n{body}");
    }
}

