using NLog;
using NLog.Web;

namespace MyNLog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");


            try
            {
                var builder = WebApplication.CreateBuilder(args);
                //使用nlog日志
                builder.Host.UseNLog();

                // Add services to the container.

                builder.Services.AddControllers();


                var app = builder.Build();

                // Configure the HTTP request pipeline.

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }
    }
}
