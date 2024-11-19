using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Controllers
{


[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly SmtpEmailService _emailService;

    public NotificationsController(SmtpEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] NotificationRequest request)
    {
        try
        {
            await _emailService.SendEmailAsync(request.To, request.Subject, request.Body);
            return Ok("Email sent successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}

}