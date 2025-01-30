using Domain.Entities;
using System.Net;
using System.Net.Mail;

namespace SendAppGI.Services
{
    public class MailService
    {
        private readonly DataStoreService _dataStoreService;
        

        // Construtor
        public MailService(DataStoreService dataStoreService)
        {
            _dataStoreService = dataStoreService;            
        }
        public async Task<bool> SendMail(string email, string password, string file, string storeName)
        {
            try
            {
                using (SmtpClient smtpClient = new("smtp.deco.com.br"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential(email, password);
                    smtpClient.EnableSsl = false;

                    using (MailMessage mail = new())
                    {
                        mail.From = new MailAddress(email);
                        mail.Subject = storeName + DateTime.Now.ToString("MM-yyyy");
                        mail.Body = "Arquivos de fechamento.";
                        mail.IsBodyHtml = false;
                        mail.To.Add("fiscal@deco.com.br");

                        if (!File.Exists(file))
                        {
                            MessageBox.Show("Arquivo não encontrado: " + file);
                            return false;
                        }

                        using (Attachment attachment = new(file))
                        {
                            mail.Attachments.Add(attachment);

                            // Log de início do envio
                            Log log = new()
                            {
                                StoreName = storeName,
                                Message = "Iniciando o envio do E-mail",
                                Created = DateTime.Now
                            };
                            await _dataStoreService.PostLogAsync(log);

                            // Envia o e-mail
                            await smtpClient.SendMailAsync(mail);

                            // Log de sucesso
                            Log logSend = new()
                            {
                                StoreName = storeName,
                                Message = "E-mail enviado!",
                                Created = DateTime.Now
                            };
                            await _dataStoreService.PostLogAsync(logSend);

                            return true;
                        }
                    }
                }
            }
            catch (SmtpException ex)
            {
                MessageBox.Show($"Erro ao enviar e-mail: {ex.Message}");
                if (ex.InnerException != null)
                {
                    MessageBox.Show($"Detalhes da exceção interna: {ex.InnerException.Message}");
                }
                return false;
            }
        }

    }
}
