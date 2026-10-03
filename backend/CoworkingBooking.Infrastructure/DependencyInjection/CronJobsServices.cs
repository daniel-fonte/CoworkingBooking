using CoworkingBooking.Infraestructure.CronJobs;
using Hangfire;

namespace CoworkingBooking.Infraestructure.DependencyInjection
{
    public static class CronJobsServices
    {
        public static void AddCronJobs()
        {
            RecurringJob.AddOrUpdate<CleanWorkspaceCalendarRecurrencesCronJob>(
                "recurrence-cleanup",
                job => job.ExecuteAsync(),
                "0 23 * * *"
            );
        }
    }
}