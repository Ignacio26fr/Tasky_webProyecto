using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;


namespace Tasky.Logica;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);

    Task SendConfirmationEmailAsync(string email, string callbackUrl);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuracion;


    public EmailService(IConfiguration configuracion)
    {
        _configuracion = configuracion;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpSettings = _configuracion.GetSection("SmtpSettings");
        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpSettings["User"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        using (var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"])))
        {
            client.Credentials = new NetworkCredential(smtpSettings["User"], smtpSettings["Password"]);
            client.EnableSsl = bool.Parse(smtpSettings["EnableSSL"]);

            await client.SendMailAsync(mailMessage);
        }
    }

    public async Task SendConfirmationEmailAsync(string email, string callbackUrl)
    {
        await SendEmailAsync(email, 
                            "Confirma tu correo",
                            $"Por favor confirma tu cuenta haciendo clic aquí: <a href='{callbackUrl}'>enlace</a>");
    }
}
