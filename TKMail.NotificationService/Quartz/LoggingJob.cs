using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKMail.NotificationService.Quartz
{
    public class LoggingJob : IJob
    {
        //ReportRepository scr = new ReportRepository();

        public async Task Execute(IJobExecutionContext context)
        {
            //await scr.Notificar();
            await Console.Out.WriteLineAsync("Greetings from HelloJob!");
            //await Console.WriteLine("Hola");

        }
    }
}
