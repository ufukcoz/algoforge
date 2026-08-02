namespace AlgoForge.Application.Common.Interfaces;

// Bu arayuz sayesinde Resend'den baska bir email saglayicisina (SendGrid, Postmark vb.)
// gecmek istersek sadece Infrastructure'daki implementasyonu degistirmemiz yeterli.
public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken);
}
