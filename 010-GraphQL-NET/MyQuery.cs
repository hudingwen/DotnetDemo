using GraphQL.Types;

namespace _010_GraphQL_NET
{
    public class MyQuery : ObjectGraphType
    {
        public MyQuery()
        {
            Field<StringGraphType>(
                "hello",
                resolve: context => "Hello, World!"
            );
        }
    }
}
