using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

   public class SmtpEmailService
   {
       private readonly SmtpSettings _smtpSettings;

       public SmtpEmailService(IOptions<SmtpSettings> smtpSettings)
       {
           _smtpSettings = smtpSettings.Value;
       }

       public async Task SendEmailAsync(string to, string subject, string body)
       {
           var email = new MimeMessage();
           email.From.Add(new MailboxAddress("Notification Service", _smtpSettings.SenderEmail));
           email.To.Add(new MailboxAddress("", to));
           email.Subject = subject;
           email.Body = new TextPart("html") { Text = body };

           using var smtp = new SmtpClient();
           await smtp.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
           await smtp.AuthenticateAsync(_smtpSettings.SenderEmail, _smtpSettings.SenderPassword);
           await smtp.SendAsync(email);
           await smtp.DisconnectAsync(true);
       }
   }

   public class SmtpSettings
   {
       public string Server { get; set; }
       public int Port { get; set; }
       public string SenderEmail { get; set; }
       public string SenderPassword { get; set; }
   }