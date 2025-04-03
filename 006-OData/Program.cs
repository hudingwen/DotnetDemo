using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace _006_OData
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //builder.Services.AddControllers();

            builder.Services.AddControllers()
    .AddOData(options => options
        .Filter()
        .Select()
        .Expand()
        .OrderBy()
        .Count()
        .SetMaxTop(100)
        .AddRouteComponents("odata", GetEdmModel()));

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Person>("People");
            return builder.GetEdmModel();
        }
    }

}
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}
