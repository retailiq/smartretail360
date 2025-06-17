using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SmartRetail360.Notifications.Interfaces.Configuration;
using SmartRetail360.Shared.Enums;

namespace SmartRetail360.Notifications.Services.Configuration;

public class MailKitEmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    private readonly IEmailTemplateProvider _templateProvider;

    public MailKitEmailSender(IConfiguration config, IEmailTemplateProvider templateProvider)
    {
        _config = config;
        _templateProvider = templateProvider;
    }

    public async Task SendAsync(string to, EmailTemplate template, Dictionary<string, string> variables)
    {
        var subject = _templateProvider.GetSubject(template);
        var bodyHtml = _templateProvider.GetBodyHtml(template, variables);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_config["App:ProductName"], _config["Mail:Smtp:Sender"]));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = bodyHtml };

        using var client = new SmtpClient();
        await client.ConnectAsync(_config["Mail:Smtp:Host"], int.Parse(_config["Mail:Smtp:Port"]!),
            MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_config["Mail:Smtp:User"], _config["Mail:Smtp:Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}