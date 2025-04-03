using Microsoft.AspNetCore.Mvc;

namespace _011_ObjectMapping.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {

        [HttpGet]
        public object Get()
        {
            return null;
        }
    }
}
