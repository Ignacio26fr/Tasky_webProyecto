using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;


namespace Tasky.Logica
{
    public class EmailService
    {
        private readonly IConfiguration _configuracion;


        public EmailService(IConfiguration configuracion)
        {
            _configuracion = configuracion;
        }

        public async Task EnviarEmailAsync(string toEmail, string subject, string body)
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
    }
}
