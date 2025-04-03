using GraphQL;
using GraphQL.Types;

namespace _010_GraphQL_NET
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 添加 GraphQL 相关服务
            builder.Services.AddScoped<ISchema, MySchema>(); // 定义 GraphQL Schema
            builder.Services.AddGraphQL(options =>
            {
                options.AddSystemTextJson();
            });

            var app = builder.Build();

            // 配置中间件以处理 GraphQL 请求
            app.UseGraphQL<ISchema>("/graphql");
            app.UseGraphQLPlayground("/ui/playground"); // GraphQL Playground

            app.Run();
        }
    }
}
