using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace SILDMS.Web.UI.ScheduledTasks
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<DestroyPolicyJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(Convert.ToInt32(WebConfigurationManager.AppSettings["MailSchHour"]), Convert.ToInt32(WebConfigurationManager.AppSettings["MailSchMin"])))
                  )
                .Build();

            scheduler.ScheduleJob(job, trigger);



            IJobDetail jobEmail = JobBuilder.Create<EmailNotificationJob4DU>().Build();

            ITrigger triggerEmail = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(Convert.ToInt32(WebConfigurationManager.AppSettings["MailSchHour"]), Convert.ToInt32(WebConfigurationManager.AppSettings["MailSchMin"])))
                  )
                .Build();

            scheduler.ScheduleJob(jobEmail, triggerEmail);
        }
    }
}