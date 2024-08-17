using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleMailSender.Dto;
using SimpleMailSender.Services.Interface;

namespace SimpleMailSender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailQueueService _emailQueueService;

        public EmailController(IEmailQueueService emailQueueService)
        {
            _emailQueueService = emailQueueService;
        }

        /// <summary>
        /// Send an email
        /// </summary>
        /// <param name="emailRequest"></param>
        /// <returns></returns>
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
        {
            await _emailQueueService.QueueEmailAsync(emailRequest.To, emailRequest.Subject, emailRequest.Body, emailRequest.IsHtml);
            return Ok("Email has been queued for sending.");
        }
    }
}
