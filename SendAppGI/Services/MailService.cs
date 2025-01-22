using Domain.Entities;
using System.Net;
using System.Net.Mail;

namespace SendAppGI.Services
{
    public class MailService(DataStoreService dataStoreService, FileService fileService)
    {
        private readonly DataStoreService _dataStoreService = dataStoreService;
        private readonly FileService _fileService = fileService;
        public async Task SendMail(string email, string password, string file, string storeName)
        {
            try
            {
                SmtpClient smtpClient = new("smtp.deco.com.br")
                {
                    Port = 0,
                    Credentials = new NetworkCredential(email, password),
                    EnableSsl = false,
                };

                MailMessage mail = new()
                {
                    From = new MailAddress(email),
                    Subject = storeName + DateTime.Now.ToString("MM-yyyy"),
                    Body = "Arquivos de fechamento.",
                    IsBodyHtml = false
                };

                //mail.To.Add("emailteste@com.br");

                mail.To.Add("fiscal@deco.com.br");

                Attachment attachment = new(file);
                mail.Attachments.Add(attachment);
                Log log = new()
                {
                    StoreName = storeName,
                    Message = "Iniciando o envio do E-mail",
                    Created = DateTime.Now
                };
                await _dataStoreService.PostLogAsync(log);
                smtpClient.Send(mail);
                log.Message = "E-mail enviado!";
                await _dataStoreService.PostLogAsync(log);
                Console.WriteLine("E-mail enviado com sucesso!");
                _fileService.DeleteZipFile(file);
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
