using System.Net.Mail;
using System.Net;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace Service
{
    public class MailService
    {
        public void SendMail(string email, string password, string file)
        {
            try
            {
                SmtpClient smtpClient = new("smtp.deco.com.br")
                {
                    Port = 587,
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

                mail.To.Add("fiscal@deco.com.br");
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

        public void GetFileForPath(string path)
        {
            string fileZip = @"C:\\Users\\Usuário\\Desktop\\Nova pasta\\Arquivo.zip";

            try
            {
                var filterFiles = Directory.EnumerateFiles(path, "*.xml", SearchOption.AllDirectories)
                    .Where(f =>
                    {
                        //DateTime now = DateTime.Now;
                        DateTime fileCreation = File.GetLastWriteTime(f);
                        DateTime now = DateTime.Now;
                        if ((now.Month == 1 && fileCreation.Month == 12) && (now.Year > fileCreation.Year) )
                        {
                            return f.EndsWith(".xml");
                        }
                        
                        return f.EndsWith(".xml") 
                               && fileCreation.Month == now.AddMonths(-1).Month;
                    });

                using (var archive = ArchiveFactory.Create(ArchiveType.Zip))
                {
                    foreach (var file in filterFiles)
                    {
                        // Adicionando o arquivo diretamente ao arquivo Zip
                        archive.AddEntry(Path.GetFileName(file), file);
                        
                    }

                    using (var stream = File.OpenWrite(fileZip))
                    {
                        archive.SaveTo(stream, CompressionType.None);
                    }

                    SendMail("lucas.feitosa@deco.com.br", "15975323Lu.", fileZip);
                }

                Console.WriteLine("Arquivos .zip criados com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler arquivos: {ex.Message}"); // Corrigido para exibir a exceção corretamente
            }
        }
    }
}
