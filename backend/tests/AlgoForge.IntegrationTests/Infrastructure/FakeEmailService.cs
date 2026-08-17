using AlgoForge.Application.Common.Interfaces;

namespace AlgoForge.IntegrationTests.Infrastructure;

public sealed class FakeEmailService : IEmailService
{
    public List<SentEmail> SentEmails { get; } = new();

    public Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken)
    {
        SentEmails.Add(
            new SentEmail(
                toEmail,
                subject,
                htmlBody));

        return Task.CompletedTask;
    }

    public record SentEmail(
        string ToEmail,
        string Subject,
        string HtmlBody);
}
