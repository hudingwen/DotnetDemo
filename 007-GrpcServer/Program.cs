namespace _007_GrpcServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddGrpc();

            //builder.Services.AddControllers();

            var app = builder.Build();

            //app.UseAuthorization();
            //app.MapControllers();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();
            });

            app.Run();
        }
    }
}
