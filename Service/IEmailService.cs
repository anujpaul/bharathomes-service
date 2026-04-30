public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody);
}

// Stub — replace with real SMTP/SendGrid impl
public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;
    public ConsoleEmailService(ILogger<ConsoleEmailService> logger) => _logger = logger;

    public Task SendAsync(string to, string subject, string htmlBody)
    {
        _logger.LogInformation("[EMAIL] To: {To} | Subject: {Subject} | Body: {Body}", to, subject, htmlBody);
        return Task.CompletedTask;
    }
}