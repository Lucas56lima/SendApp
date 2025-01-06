﻿using System.Net.Mail;
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
<<<<<<< HEAD
                SmtpClient smtpClient = new("***.com.br")
=======
                SmtpClient smtpClient = new("smtp.com.br")
>>>>>>> 51bee8023b0dceecf71a8d930cf1687e7689de2a
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

<<<<<<< HEAD
                mail.To.Add("emailteste@com.br");
=======
                mail.To.Add("destinatario@deco.com.br");
>>>>>>> 51bee8023b0dceecf71a8d930cf1687e7689de2a
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
            string fileZip = @"caminho da pasta";

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

<<<<<<< HEAD
                    Console.WriteLine("Arquivos .zip criados com sucesso!");
=======
                    SendMail("seuemail@deco.com.br", "password".", fileZip);
                }
>>>>>>> 51bee8023b0dceecf71a8d930cf1687e7689de2a

                    SendMail("seuemail@dcom.br", "sua senha", fileZip);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler arquivos: {ex.Message}"); // Corrigido para exibir a exceção corretamente
            }
        }
    }
}
