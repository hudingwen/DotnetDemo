using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
 

namespace _008_GrpcClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
      
        [HttpGet]
        public async Task<object> Get(string name="hudingwen")
        { 
            var channel = GrpcChannel.ForAddress("https://localhost:5044");
            var client = new  Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
            return reply.Message;
        }
    }
}
