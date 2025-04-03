using _009_GraphQL_HotChocolate.Controllers;

namespace _009_GraphQL_HotChocolate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args); 
            builder.Services.AddGraphQLServer().AddQueryType<Query>(); ; 

            var app = builder.Build();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL(); // ½« GraphQL ±©Â¶Îª API
            });

            app.Run();
        }
    }
}
