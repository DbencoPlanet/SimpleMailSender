using MimeKit.Text;
using MimeKit;
using SimpleMailSender.Services.Interface;
using System.Threading.Channels;

namespace SimpleMailSender.Services.Concrete
{
    public class EmailQueueService: IEmailQueueService
    {
        private readonly Channel<MimeMessage> _emailQueue;
        private readonly IEmailService _emailService;

        public EmailQueueService(IEmailService emailService)
        {
            _emailService = emailService;
            _emailQueue = Channel.CreateUnbounded<MimeMessage>();
        }

        public async Task QueueEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            var emailBody = isHtml ? await _emailService.BuildHtmlEmailBodyAsync(subject, body, "Your Company Name") : body;

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailService.GetSmtpSettings().FromEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(isHtml ? TextFormat.Html : TextFormat.Plain) { Text = emailBody };

            await _emailQueue.Writer.WriteAsync(email);
        }

        public async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            await foreach (var email in _emailQueue.Reader.ReadAllAsync(cancellationToken))
            {
                await _emailService.SendEmailAsync(email);
            }
        }
    }
}
