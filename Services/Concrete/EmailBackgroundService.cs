
using Microsoft.Extensions.Hosting;
using SimpleMailSender.Services.Interface;
using System.Threading;
using System.Threading.Tasks;
namespace SimpleMailSender.Services.Concrete
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IEmailQueueService _emailQueueService;

        public EmailBackgroundService(IEmailQueueService emailQueueService)
        {
            _emailQueueService = emailQueueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _emailQueueService.ProcessQueueAsync(stoppingToken);
        }
    }
}
