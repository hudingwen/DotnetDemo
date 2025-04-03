using Hangfire;
using Hangfire.MemoryStorage;

namespace _012_Task_Hangfire
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 配置 Hangfire 服务
            builder.Services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });

            // 启用 Hangfire Server
            builder.Services.AddHangfireServer();

            var app = builder.Build();

            // 配置 Hangfire Dashboard （可选，用于查看任务）
            app.UseHangfireDashboard();

            // 即时任务
            BackgroundJob.Enqueue(() => Console.WriteLine("This is an immediate background job!"));
            //延迟任务
            BackgroundJob.Schedule(() => Console.WriteLine("This will run after a delay of 1 minute!"), TimeSpan.FromMinutes(1));
            //定时任务
            RecurringJob.AddOrUpdate("daily-task", () => Console.WriteLine("Daily job executed!"), "0 9 * * *");
            //连续任务
            var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("First job"));
            BackgroundJob.ContinueWith(jobId, () => Console.WriteLine("Continued job after first job!"));


            app.Run();
        }
    }
}
