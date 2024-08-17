using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using SimpleMailSender.Dto;
using SimpleMailSender.Services.Interface;
using System.Collections.Concurrent;

namespace SimpleMailSender.Services.Concrete
{
    public class EmailService: IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ConcurrentQueue<MimeMessage> _failedEmails;
        private readonly Timer _resendTimer;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
            _failedEmails = new ConcurrentQueue<MimeMessage>();

            _resendTimer = new Timer(ResendFailedEmails, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }


        public async Task SendEmailAsync(MimeMessage email)
        {
            try
            {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception)
            {
                _failedEmails.Enqueue(email);
            }
        }

        public void ScheduleFailedEmailsResend()
        {
            _resendTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private async void ResendFailedEmails(object state)
        {
            while (_failedEmails.TryDequeue(out var failedEmail))
            {
                await SendEmailAsync(failedEmail);
            }
        }

        public async Task<string> BuildHtmlEmailBodyAsync(string subject, string body, string senderName)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate/Layout.html");
            var template = await File.ReadAllTextAsync(templatePath);

            template = template.Replace("{{Subject}}", subject)
                               .Replace("{{Title}}", subject)
                               .Replace("{{Body}}", body)
                               .Replace("{{SenderName}}", senderName);

            return template;
        }

        public SmtpSettings GetSmtpSettings()
        {
            return _smtpSettings;
        }
    }
}
