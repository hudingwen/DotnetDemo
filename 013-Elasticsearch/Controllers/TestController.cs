using Microsoft.AspNetCore.Mvc;
using Nest;

namespace _013_Elasticsearch.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController] 
    public class TestController : ControllerBase
    {

        private readonly IElasticClient _elasticClient;

        public TestController()
        {
            var settings = new ConnectionSettings(new Uri("http://121.40.205.43:50006"))
                .DefaultIndex("products").BasicAuthentication("elastic", "123456"); // 设置基本认证; // 设置默认索引名称
            _elasticClient = new ElasticClient(settings);
        }

        // 获取客户端
        public IElasticClient Client => _elasticClient;
        [HttpGet]
        // 初始化索引和写入模拟数据
        public async Task test()
        {
            // 检查索引是否存在
            var existsResponse = await _elasticClient.Indices.ExistsAsync("products");
            if (!existsResponse.Exists)
            {
                // 创建索引
                await _elasticClient.Indices.CreateAsync("products", c => c
                    .Map<Product>(m => m
                        .AutoMap()
                    )
                );
            }

            // 写入模拟数据
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "苹果15", Description = "这是一段描述1",brandNmae="苹果(Apple)", Price = 999.99m , parameters=
                    new List<Parameter>(){ new Parameter() { name ="颜色",value= "白色" },new Parameter() { name="运行内存",value="4G"}  ,new Parameter() { name="机身内存",value= "128G" } } },
                new Product { Id = 2, Name = "苹果15", Description = "这是一段描述2",brandNmae="苹果(Apple)", Price = 799.99m, parameters=
                    new List<Parameter>(){ new Parameter() { name ="颜色",value= "白色" },new Parameter() { name="运行内存",value="6G"}  ,new Parameter() { name="机身内存",value= "256G" } } },
                new Product { Id = 3, Name = "苹果15", Description = "这是一段描述3",brandNmae="苹果(Apple)", Price = 199.99m , parameters=
                    new List<Parameter>(){ new Parameter() { name ="颜色",value= "白色" },new Parameter() { name="运行内存",value="8G"}  ,new Parameter() { name="机身内存",value= "512G" } } },
                new Product { Id = 4, Name = "苹果15", Description = "这是一段描述4",brandNmae="苹果(Apple)", Price = 499.99m , parameters=
                    new List<Parameter>(){ new Parameter() { name ="颜色",value= "红色" },new Parameter() { name="运行内存",value="4G"}  ,new Parameter() { name="机身内存",value= "128G" } } },
                new Product { Id = 5, Name = "苹果15", Description = "这是一段描述5",brandNmae="苹果(Apple)", Price = 299.99m , parameters=
                    new List<Parameter>(){ new Parameter() { name ="颜色",value= "红色" },new Parameter() { name="运行内存",value="6G"}  ,new Parameter() { name="机身内存",value= "256G" } } },
                new Product { Id = 6, Name = "苹果15", Description = "这是一段描述6",brandNmae="苹果(Apple)", Price = 299.99m , parameters=
                    new List<Parameter>(){ new Parameter() { name ="颜色",value= "红色" },new Parameter() { name="运行内存",value="8G"}  ,new Parameter() { name="机身内存",value= "512G" } } },
            };

            // 批量写入数据
            var bulkResponse = await _elasticClient.BulkAsync(b => b
                .Index("products")
                .IndexMany(products)
            );

            if (bulkResponse.Errors)
            {
                throw new Exception("批量写入数据时发生错误：" + bulkResponse.ItemsWithErrors);
            }
        }
        [HttpGet]

        public async Task<List<Product>> test2(string keyword)
        {
            //var response = await _elasticClient.SearchAsync<Product>(s => s
            //    .Query(q => q
            //        .Wildcard(w => w
            //            .Field(f => f.Name)
            //            .Value($"*{keyword}*") // 使用通配符进行模糊匹配
            //        )
            //    )
            //    .Size(10) // 返回前10条记录
            //);

            var response = await _elasticClient.SearchAsync<Product>(s => s
          .Query(q => q
              .Bool(b => b
                  .Should(
                      sh => sh.Match(m => m
                          .Field(f => f.Name)
                          .Query(keyword)
                          .Fuzziness(Fuzziness.Auto) // 模糊匹配
                      ),
                      sh => sh.Wildcard(w => w
                          .Field(f => f.Name)
                          .Value($"*{keyword}*") // 使用通配符进行模糊匹配
                      ),
                      sh => sh.Wildcard(w => w
                          .Field(f => f.brandNmae)
                          .Value($"*{keyword}*") // 使用通配符进行模糊匹配
                      ),
                      sh => sh.Wildcard(w => w
                          .Field(f => f.parameters.Find(ff=>keyword.Contains(ff.value)))
                          .Value($"*{keyword}*") // 使用通配符进行模糊匹配
                      )
                  )
              )
          )
          .Size(10) // 返回前10条记录
      );

            if (!response.IsValid)
            {
                // 处理错误，例如记录日志或抛出异常
                throw new Exception($"搜索失败：{response.OriginalException.Message}");
            }

            return response.Documents.ToList();
        }
    }
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public List<Parameter> parameters;

        public string brandNmae { get; set; }

    }

    public class Parameter 
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}
