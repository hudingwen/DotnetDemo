using GraphQL;
using GraphQL.Types;

namespace _010_GraphQL_NET
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ��� GraphQL ��ط���
            builder.Services.AddScoped<ISchema, MySchema>(); // ���� GraphQL Schema
            builder.Services.AddGraphQL(options =>
            {
                options.AddSystemTextJson();
            });

            var app = builder.Build();

            // �����м���Դ��� GraphQL ����
            app.UseGraphQL<ISchema>("/graphql");
            app.UseGraphQLPlayground("/ui/playground"); // GraphQL Playground

            app.Run();
        }
    }
}
