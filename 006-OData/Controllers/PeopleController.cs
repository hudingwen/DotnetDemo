using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace _006_OData.Controllers
{
    [Route("odata/[controller]")]
    public class PeopleController : ODataController
    {
        private static List<Person> _people = new List<Person>
    {
        new Person { Id = 1, Name = "John", Age = 25 },
        new Person { Id = 2, Name = "Jane", Age = 30 },
        new Person { Id = 3, Name = "Jake", Age = 35 }
    };

        // GET /odata/People
        ///odata/People?$filter=Age gt 30&$orderby=Name desc&$select=Name,Age&$top=5&$skip=1&$count=true
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_people.AsQueryable());
        }

        // GET /odata/People(1)
        [EnableQuery]
        public IActionResult Get([FromODataUri] int key)
        {
            var person = _people.FirstOrDefault(p => p.Id == key);
            return person != null ? Ok(person) : NotFound();
        }
        //自定义查询参数
        [EnableQuery]
        public IActionResult Get(ODataQueryOptions<Person> options)
        {
            // 手动处理 OData 查询参数
            IQueryable<Person> query = _people.AsQueryable();

            // 过滤
            if (options.Filter != null)
            {
                query = options.Filter.ApplyTo(query, new ODataQuerySettings()) as IQueryable<Person>;
            }

            // 排序
            if (options.OrderBy != null)
            {
                query = options.OrderBy.ApplyTo(query, new ODataQuerySettings()) as IOrderedQueryable<Person>;
            }

            // 分页
            if (options.Skip != null)
            {
                query = options.Skip.ApplyTo(query, new ODataQuerySettings()) as IQueryable<Person>;
            }

            if (options.Top != null)
            {
                query = options.Top.ApplyTo(query, new ODataQuerySettings()) as IQueryable<Person>;
            }

            return Ok(query);
        }
    }
}
