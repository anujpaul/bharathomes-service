using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody);
}

// Stub — replace with real SMTP/SendGrid impl
//public class ConsoleEmailService : IEmailService
//{
//    private readonly ILogger<ConsoleEmailService> _logger;
//    public ConsoleEmailService(ILogger<ConsoleEmailService> logger) => _logger = logger;

//    public Task SendAsync(string to, string subject, string htmlBody)
//    {
//        _logger.LogInformation("[EMAIL] To: {To} | Subject: {Subject} | Body: {Body}", to, subject, htmlBody);
//        return Task.CompletedTask;
//    }
//}

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_config["Smtp:From"]));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            await client.ConnectAsync(_config["Smtp:Host"],
                int.Parse(_config["Smtp:Port"]!),
                SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_config["Smtp:User"], _config["Smtp:Pass"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("[EMAIL SENT] To: {To} | Subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError("[EMAIL FAILED] To: {To} | Error: {Error}", to, ex.Message);
            throw;
        }
    }
}