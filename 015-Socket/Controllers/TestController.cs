using Microsoft.AspNetCore.Mvc;

namespace _015_Socket.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : ControllerBase
    {
        

        [HttpGet]
        [Route("test")]
        public async Task<string> test()
        {
            await SocketHelper.CreateServer();
            return "OK";
        }

        [HttpGet]
        [Route("test2")]
        public async Task<string> test2(string msg)
        {
           var data =  await SocketHelper.CreateClient(msg);
            return data;
        }
    }
}
