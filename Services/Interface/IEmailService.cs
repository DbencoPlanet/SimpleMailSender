using MimeKit;
using SimpleMailSender.Dto;

namespace SimpleMailSender.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(MimeMessage email);
        Task<string> BuildHtmlEmailBodyAsync(string subject, string body, string senderName);
        SmtpSettings GetSmtpSettings();
        void ScheduleFailedEmailsResend();
    }
}
