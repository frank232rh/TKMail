using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKMail.Data.Abstract;
using TKMail.Data.Concrete;

namespace TKMail.NotificationService.Quartz
{
    public class JobScheduler
    {
        private readonly iMailRepository repository;
        public JobScheduler()
        {
            repository = new MailRepository();
        }
        public async void Start()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();


            IJobDetail job = JobBuilder.Create<LoggingJob>().Build();

            //string strCronExpressionNotificar= "0 45 11 ? * * *";
            string strCronExpressionNotificar = repository.GetGeneralConfiguration("CronExpressionNotificar").Trim();//At 15:05:00pm every day
            if (String.IsNullOrEmpty(strCronExpressionNotificar))
            {
                strCronExpressionNotificar = "0 5 15 ? * * *";
            }
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("Notificar", "GreetingGroup")
                //.WithSimpleSchedule(x => x
                //    .WithIntervalInHours(24)
                //    //.WithIntervalInSeconds(10)
                //    .RepeatForever())
                //.StartNow()
                .WithCronSchedule(strCronExpressionNotificar)
                .WithPriority(1)
                .Build();

            await scheduler.ScheduleJob(job, trigger);

        }
    }
}
