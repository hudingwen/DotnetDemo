using Hangfire;
using Hangfire.MemoryStorage;

namespace _012_Task_Hangfire
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ���� Hangfire ����
            builder.Services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });

            // ���� Hangfire Server
            builder.Services.AddHangfireServer();

            var app = builder.Build();

            // ���� Hangfire Dashboard ����ѡ�����ڲ鿴����
            app.UseHangfireDashboard();

            // ��ʱ����
            BackgroundJob.Enqueue(() => Console.WriteLine("This is an immediate background job!"));
            //�ӳ�����
            BackgroundJob.Schedule(() => Console.WriteLine("This will run after a delay of 1 minute!"), TimeSpan.FromMinutes(1));
            //��ʱ����
            RecurringJob.AddOrUpdate("daily-task", () => Console.WriteLine("Daily job executed!"), "0 9 * * *");
            //��������
            var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("First job"));
            BackgroundJob.ContinueWith(jobId, () => Console.WriteLine("Continued job after first job!"));


            app.Run();
        }
    }
}
