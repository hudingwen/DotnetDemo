using Gridify;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace _005_Gridify.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<Person> Get()
        {
            var people = new List<Person>
{
                new Person { Id = 1, Name = "John", Age = 25 },
                new Person { Id = 2, Name = "Jane", Age = 30 },
                new Person { Id = 3, Name = "Jake", Age = 35 },
                new Person { Id = 4, Name = "Anna", Age = 28 }
            }.AsQueryable();

            ////查询
            //// 设定查询字符串
            //string filter = "Name = John";
            //// 使用 Gridify 进行查询
            //var result = people.ApplyFiltering(filter).ToList();

            ////分页
            //int pageNumber = 1;
            //int pageSize = 2;
            //var result = people.ApplyPaging(pageNumber, pageSize).ToList();

            ////排序
            //string sorting = "Age desc";
            //var result = people.ApplyOrdering(sorting).ToList();

            ////综合查询
            //string filter = "Age > 25";
            //string sorting = "Age desc";
            //int pageNumber = 1;
            //int pageSize = 2;
            //var result = people
            //    .ApplyFiltering(filter)
            //    .ApplyOrdering(sorting)
            //    .ApplyPaging(pageNumber, pageSize)
            //    .ToList();


            //LINQ Lambda 表达式
            string filter = "Age > 25";
            string order = "Age desc";
            var expressionFilter = (new GridifyQuery(1, 10, filter, order)).GetFilteringExpression<Person>();
            var expressionOrder = (new GridifyQuery(1, 10, filter,order)).GetOrderingExpressions<Person>();
            var result =  people.Where(expressionFilter).OrderByDescending(expressionOrder.First()).ToList();
            // 输出结果
            return result;
        }
    }


    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
