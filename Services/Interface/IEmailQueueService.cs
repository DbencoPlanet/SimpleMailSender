namespace SimpleMailSender.Services.Interface
{
    public interface IEmailQueueService
    {
        Task QueueEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task ProcessQueueAsync(CancellationToken cancellationToken);
    }
}
