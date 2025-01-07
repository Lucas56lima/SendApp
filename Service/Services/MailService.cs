using System.Net.Mail;
using System.Net;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace Service.Services
{
    public class MailService
    {
        public void SendMail(string email, string password, string file)
        {
            try
            {
                SmtpClient smtpClient = new("smtp.com.br")
                {
                    Port = 0,
                    Credentials = new NetworkCredential(email, password),
                    EnableSsl = false,
                };

                MailMessage mail = new()
                {
                    From = new MailAddress(email),
                    Subject = "Teste",
                    Body = "Outro teste",
                    IsBodyHtml = false
                };


                mail.To.Add("emailteste@com.br");

                mail.To.Add("destinatario@deco.com.br");

                Attachment attachment = new(file);
                mail.Attachments.Add(attachment);
                smtpClient.Send(mail);

                Console.WriteLine("E-mail enviado com sucesso!");
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"Erro SMTP: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Detalhes da exceção interna: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Detalhes da exceção interna: {ex.InnerException.Message}");
                }
            }
        }
        
    }
}
