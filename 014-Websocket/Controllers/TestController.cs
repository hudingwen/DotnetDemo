using Microsoft.AspNetCore.Mvc;

namespace _014_Websocket.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : ControllerBase
    { 

        [HttpGet]
        [Route("test")]
        public async Task<string> GetAsync(string userId,string content)
        {
            await WebSocketHandlerMiddleware.SendMessageToClient(userId, content);
            return "OK";

        }
    }
}
