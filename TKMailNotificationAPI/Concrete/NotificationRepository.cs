using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using TKMail.Data.Entities;
using MailNotificationAPI.Abstract;
using MailNotificationAPI.Helpers;

namespace MailNotificationAPI.Concrete
{
    public class NotificationRepository : iNotificationRepository
    {
        public int SendMail(eMailMessage eMailMessage, MailConfig mailConfig)
        {
            int response = 0;
            try
            {
                #region Production
                SmtpClient smptClient = new SmtpClient(mailConfig.Host, mailConfig.Port);
                //credentials to login in to hotmail account

                //DEBUG
                //smptClient.Credentials = new NetworkCredential(mailConfig.EmailAddress, mailConfig.Password); //DESCOMENTAR PARA PRUEBAS

                //PROD
                smptClient.Credentials = new NetworkCredential(mailConfig.EmailAddress, mailConfig.Password); //PRODUCCION

                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(mailConfig.EmailAddress, mailConfig.Subject);

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(eMailMessage.TemplateHTML, Encoding.UTF8, MediaTypeNames.Text.Html);
                mail.AlternateViews.Add(htmlView);
                mail.Body = eMailMessage.TemplateHTML;
                mail.IsBodyHtml = true;

                mail.Subject = mailConfig.Subject;
                if (eMailMessage.Attachments.Count > 0)
                {
                    foreach (var item in eMailMessage.Attachments)
                    {
                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(new MemoryStream(item.File), item.FileName);
                        mail.Attachments.Add(attachment);
                    }
                    //if (eMailMessage.Attachment.Length > 0)
                    //{
                    //    var cadena = eMailMessage.Attachment.Substring(eMailMessage.Attachment.LastIndexOf(',') + 1);
                    //    byte[] bFile = Convert.FromBase64String(cadena);

                    //    System.Net.Mail.Attachment attachment;
                    //    attachment = new System.Net.Mail.Attachment(new MemoryStream(bFile), eMailMessage.FileName);
                    //    mail.Attachments.Add(attachment);
                    //}
                }

                var destinatarios = eMailMessage.MailTo.Split(';');

                foreach (var item in destinatarios)
                {
                    if (item != "")
                    {
                        mail.To.Add(item);
                    }
                }

                //mail.To.Add(eMailMessage.to);
                if (mail.To.Count > 0)
                {
                    //Console.WriteLine($"Enviando email a {mail.To}...");
                    smptClient.EnableSsl = mailConfig.IsEnableSSL;
                    smptClient.Send(mail);
                    response = 1;
                    //Console.WriteLine($"El email se envio correctamente.");
                }
                #endregion

            }
            catch (Exception ex)
            {
                Models.Extensions.StringExtensions.createLog(ex.Message, "SendMail", "36");
                LogMethods.writeException(ex, "TKMail.NotificationAPI.Concrete.NotificationRepository.SendMail");
            }
            finally
            {
                GC.Collect();
            }
            return response;
        }

    }
}