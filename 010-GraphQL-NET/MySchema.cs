using GraphQL.Types;

namespace _010_GraphQL_NET
{
    public class MySchema : Schema
    {
        public MySchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<MyQuery>();  
        }
    }
}
