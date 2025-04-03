using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

namespace MySerilog
{
    public class Program
    {
        public static void Main(string[] args)
        {

            // 配置列选项 - 你可以指定额外的列
            var columnOptions = new ColumnOptions();
            columnOptions.Store.Clear();
            //系统列
            columnOptions.Store.Add(StandardColumn.Id);
            columnOptions.Store.Add(StandardColumn.Level);
            columnOptions.Store.Add(StandardColumn.TimeStamp);
            columnOptions.Store.Add(StandardColumn.Message);
            columnOptions.Store.Add(StandardColumn.Exception);
            //不需要的系统列
            columnOptions.Store.Remove(StandardColumn.Properties); // 不保存属性列
            columnOptions.Store.Remove(StandardColumn.LogEvent);      // 保存LogEvent列（包含原始JSON）
            // 添加自定义列
            columnOptions.AdditionalColumns = new Collection<SqlColumn>
            {
                // 自定义字段 - 请求的唯一 ID
                new SqlColumn("RequestId", SqlDbType.NVarChar, dataLength: 50),
                
                // 自定义字段 - 用户 ID
                new SqlColumn("UserId", SqlDbType.NVarChar, dataLength: 50),

                // 自定义字段 - 自定义日志类型
                new SqlColumn("LogType", SqlDbType.NVarChar, dataLength: 50)
            };

            //1.配置方式
            if (true)
            {
                var configuration = new ConfigurationBuilder()
                  .AddJsonFile("appsettings.json")
                  .Build();
                Log.Logger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(
                 connectionString: configuration.GetConnectionString("DefaultConnection"),
                sinkOptions: new MSSqlServerSinkOptions
                {
                    TableName = "CustomLogs",
                    AutoCreateSqlTable = true
                },
                columnOptions: columnOptions)  // 应用自定义的 ColumnOptions
               .ReadFrom.Configuration(configuration) // 从配置文件中读取Serilog配置
               
               .CreateLogger();
            }


            //2.代码方式
            if (false)
            {
                Log.Logger = new LoggerConfiguration()
                //过滤嘈杂日志
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
                //控制台日志
                .WriteTo.Console()
                 //文件日志
                 .WriteTo.File("logs/log-.txt",
                        rollingInterval: RollingInterval.Day,  // 每天创建一个新日志文件
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}") // 日志格式
                                                                                                                         //数据库日志
                 .WriteTo.MSSqlServer(
                        connectionString: "Data Source = 121.40.205.43,50004;Initial Catalog = logdatabase;User Id = sa;Password = @#$.;Encrypt=False;", // 数据库连接字符串
                        sinkOptions: new MSSqlServerSinkOptions
                        {
                            AutoCreateSqlTable = true, // 自动创建表
                            TableName = "Logs"         // 表名
                        },
                        columnOptions: columnOptions,  // 可选列配置
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information) // 设置最低记录级别
                 .CreateLogger();
            }



            Log.Information("Starting web application");

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                //添加日志记录器
                builder.Services.AddSerilog();
                // Add services to the container.

                builder.Services.AddControllers();

                var app = builder.Build();

                // 注册自定义的请求-响应记录中间件
                app.UseMiddleware<RequestResponseLoggingMiddleware>();

                // Configure the HTTP request pipeline.

                app.UseAuthorization();


                app.MapControllers();

                //serilog默认http日志请求
                app.UseSerilogRequestLogging();
                //serilog自定义http日志请求
                //app.UseSerilogRequestLogging(options =>
                //{
                //    // Customize the message template
                //    options.MessageTemplate = "Handled {RequestPath}";

                //    // Emit debug-level events instead of the defaults
                //    options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;

                //    // Attach additional properties to the request completion event
                //    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                //    {
                //        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                //        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                //    };
                //});
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
