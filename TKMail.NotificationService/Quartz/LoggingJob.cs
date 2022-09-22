using MailNotificationAPI.Concrete;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKMail.Data.Concrete;
using TKMail.Data.Entities;

namespace TKMail.NotificationService.Quartz
{
    public class LoggingJob : IJob
    {
        MailRepository scr = new MailRepository();
        NotificationRepository notiRepository = new NotificationRepository();

        public async Task Execute(IJobExecutionContext context)
        {
            List<eMailMessage> notSendend =  scr.GetNotSentNotification();
            if (notSendend.Count > 0)
            {
                int res = 0;
                foreach (eMailMessage message in notSendend)
                {
                    //eMailMessage model = repository.GetMailNotification(int.Parse(Id));
                    MailConfig mailConfig = scr.GetMailConfig(message.IdApp);

                    if (message != null && !message.Sent)
                    {
                        res = await message.sendMail(message, mailConfig);
                        if (res != 1)
                        {
                            //("Result was distinct to 1 " + res.ToString() + " | The result was reenqueue").createLog("QueueProcessor - Work ", "53");
                            //EnqueueOrdId(Id);
                            var x = scr.UpdateMail(message.Id, false);
                        }
                        else
                        {
                            var x = scr.UpdateMail(message.Id, true);
                            //int x = repository.updateMessage(Id);
                        }
                    }
                }
            }
            await Console.Out.WriteLineAsync("Greetings from HelloJob!");
            //await Console.WriteLine("Hola");

        }
    }
}
