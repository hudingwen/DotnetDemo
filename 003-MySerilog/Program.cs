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

            // ������ѡ�� - �����ָ���������
            var columnOptions = new ColumnOptions();
            columnOptions.Store.Clear();
            //ϵͳ��
            columnOptions.Store.Add(StandardColumn.Id);
            columnOptions.Store.Add(StandardColumn.Level);
            columnOptions.Store.Add(StandardColumn.TimeStamp);
            columnOptions.Store.Add(StandardColumn.Message);
            columnOptions.Store.Add(StandardColumn.Exception);
            //����Ҫ��ϵͳ��
            columnOptions.Store.Remove(StandardColumn.Properties); // ������������
            columnOptions.Store.Remove(StandardColumn.LogEvent);      // ����LogEvent�У�����ԭʼJSON��
            // ����Զ�����
            columnOptions.AdditionalColumns = new Collection<SqlColumn>
            {
                // �Զ����ֶ� - �����Ψһ ID
                new SqlColumn("RequestId", SqlDbType.NVarChar, dataLength: 50),
                
                // �Զ����ֶ� - �û� ID
                new SqlColumn("UserId", SqlDbType.NVarChar, dataLength: 50),

                // �Զ����ֶ� - �Զ�����־����
                new SqlColumn("LogType", SqlDbType.NVarChar, dataLength: 50)
            };

            //1.���÷�ʽ
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
                columnOptions: columnOptions)  // Ӧ���Զ���� ColumnOptions
               .ReadFrom.Configuration(configuration) // �������ļ��ж�ȡSerilog����
               
               .CreateLogger();
            }


            //2.���뷽ʽ
            if (false)
            {
                Log.Logger = new LoggerConfiguration()
                //����������־
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
                //����̨��־
                .WriteTo.Console()
                 //�ļ���־
                 .WriteTo.File("logs/log-.txt",
                        rollingInterval: RollingInterval.Day,  // ÿ�촴��һ������־�ļ�
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}") // ��־��ʽ
                                                                                                                         //���ݿ���־
                 .WriteTo.MSSqlServer(
                        connectionString: "Data Source = 121.40.205.43,50004;Initial Catalog = logdatabase;User Id = sa;Password = @#$.;Encrypt=False;", // ���ݿ������ַ���
                        sinkOptions: new MSSqlServerSinkOptions
                        {
                            AutoCreateSqlTable = true, // �Զ�������
                            TableName = "Logs"         // ����
                        },
                        columnOptions: columnOptions,  // ��ѡ������
                        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information) // ������ͼ�¼����
                 .CreateLogger();
            }



            Log.Information("Starting web application");

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                //�����־��¼��
                builder.Services.AddSerilog();
                // Add services to the container.

                builder.Services.AddControllers();

                var app = builder.Build();

                // ע���Զ��������-��Ӧ��¼�м��
                app.UseMiddleware<RequestResponseLoggingMiddleware>();

                // Configure the HTTP request pipeline.

                app.UseAuthorization();


                app.MapControllers();

                //serilogĬ��http��־����
                app.UseSerilogRequestLogging();
                //serilog�Զ���http��־����
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
