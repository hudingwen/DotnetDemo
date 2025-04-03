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
                .DefaultIndex("products").BasicAuthentication("elastic", "123456"); // ���û�����֤; // ����Ĭ����������
            _elasticClient = new ElasticClient(settings);
        }

        // ��ȡ�ͻ���
        public IElasticClient Client => _elasticClient;
        [HttpGet]
        // ��ʼ��������д��ģ������
        public async Task test()
        {
            // ��������Ƿ����
            var existsResponse = await _elasticClient.Indices.ExistsAsync("products");
            if (!existsResponse.Exists)
            {
                // ��������
                await _elasticClient.Indices.CreateAsync("products", c => c
                    .Map<Product>(m => m
                        .AutoMap()
                    )
                );
            }

            // д��ģ������
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "ƻ��15", Description = "����һ������1",brandNmae="ƻ��(Apple)", Price = 999.99m , parameters=
                    new List<Parameter>(){ new Parameter() { name ="��ɫ",value= "��ɫ" },new Parameter() { name="�����ڴ�",value="4G"}  ,new Parameter() { name="�����ڴ�",value= "128G" } } },
                new Product { Id = 2, Name = "ƻ��15", Description = "����һ������2",brandNmae="ƻ��(Apple)", Price = 799.99m, parameters=
                    new List<Parameter>(){ new Parameter() { name ="��ɫ",value= "��ɫ" },new Parameter() { name="�����ڴ�",value="6G"}  ,new Parameter() { name="�����ڴ�",value= "256G" } } },
                new Product { Id = 3, Name = "ƻ��15", Description = "����һ������3",brandNmae="ƻ��(Apple)", Price = 199.99m , parameters=
                    new List<Parameter>(){ new Parameter() { name ="��ɫ",value= "��ɫ" },new Parameter() { name="�����ڴ�",value="8G"}  ,new Parameter() { name="�����ڴ�",value= "512G" } } },
                new Product { Id = 4, Name = "ƻ��15", Description = "����һ������4",brandNmae="ƻ��(Apple)", Price = 499.99m , parameters=
                    new List<Parameter>(){ new Parameter() { name ="��ɫ",value= "��ɫ" },new Parameter() { name="�����ڴ�",value="4G"}  ,new Parameter() { name="�����ڴ�",value= "128G" } } },
                new Product { Id = 5, Name = "ƻ��15", Description = "����һ������5",brandNmae="ƻ��(Apple)", Price = 299.99m , parameters=
                    new List<Parameter>(){ new Parameter() { name ="��ɫ",value= "��ɫ" },new Parameter() { name="�����ڴ�",value="6G"}  ,new Parameter() { name="�����ڴ�",value= "256G" } } },
                new Product { Id = 6, Name = "ƻ��15", Description = "����һ������6",brandNmae="ƻ��(Apple)", Price = 299.99m , parameters=
                    new List<Parameter>(){ new Parameter() { name ="��ɫ",value= "��ɫ" },new Parameter() { name="�����ڴ�",value="8G"}  ,new Parameter() { name="�����ڴ�",value= "512G" } } },
            };

            // ����д������
            var bulkResponse = await _elasticClient.BulkAsync(b => b
                .Index("products")
                .IndexMany(products)
            );

            if (bulkResponse.Errors)
            {
                throw new Exception("����д������ʱ��������" + bulkResponse.ItemsWithErrors);
            }
        }
        [HttpGet]

        public async Task<List<Product>> test2(string keyword)
        {
            //var response = await _elasticClient.SearchAsync<Product>(s => s
            //    .Query(q => q
            //        .Wildcard(w => w
            //            .Field(f => f.Name)
            //            .Value($"*{keyword}*") // ʹ��ͨ�������ģ��ƥ��
            //        )
            //    )
            //    .Size(10) // ����ǰ10����¼
            //);

            var response = await _elasticClient.SearchAsync<Product>(s => s
          .Query(q => q
              .Bool(b => b
                  .Should(
                      sh => sh.Match(m => m
                          .Field(f => f.Name)
                          .Query(keyword)
                          .Fuzziness(Fuzziness.Auto) // ģ��ƥ��
                      ),
                      sh => sh.Wildcard(w => w
                          .Field(f => f.Name)
                          .Value($"*{keyword}*") // ʹ��ͨ�������ģ��ƥ��
                      ),
                      sh => sh.Wildcard(w => w
                          .Field(f => f.brandNmae)
                          .Value($"*{keyword}*") // ʹ��ͨ�������ģ��ƥ��
                      ),
                      sh => sh.Wildcard(w => w
                          .Field(f => f.parameters.Find(ff=>keyword.Contains(ff.value)))
                          .Value($"*{keyword}*") // ʹ��ͨ�������ģ��ƥ��
                      )
                  )
              )
          )
          .Size(10) // ����ǰ10����¼
      );

            if (!response.IsValid)
            {
                // ������������¼��־���׳��쳣
                throw new Exception($"����ʧ�ܣ�{response.OriginalException.Message}");
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
