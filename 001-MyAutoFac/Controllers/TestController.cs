using Autofac;
using Autofac.Core.Lifetime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MyAutoFac.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        public IConfiguration _configuration { get; set; }
        public IUserService _userService { get; set; }


        public IConfiguration configuration2;
        public IUserService userService2;


        public TestController(IConfiguration configuration, IUserService userService)
        {
            configuration2 = configuration;
            userService2 = userService;
        }
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("test")]
        public string test()
        {
            return _userService.say();
        }
        /// <summary>
        /// 属性注入
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("test2")]
        public string test2()
        {
            return _userService.say2();
        }
        /// <summary>
        /// 方法注入
        /// </summary>
        [HttpGet]
        [Route("test3")]
        public void test3()
        {
            using (var scope = ContainerHelper.container.BeginLifetimeScope())
            {
                var userService = scope.Resolve<IUserService>();
                userService.say();
                userService.say();
            }

        }
         
    }
}
