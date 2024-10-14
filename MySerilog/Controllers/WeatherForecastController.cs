using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MySerilog.Controllers
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
        public IEnumerable<WeatherForecast> Get()
        {
            //记录多用户日志
            Log.ForContext("RequestId", Guid.NewGuid().ToString())
               .ForContext("UserId",  Guid.NewGuid().ToString())
               .ForContext("LogType", Guid.NewGuid().ToString())
               .Information("This is a custom log message");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpPost]
        public IEnumerable<WeatherForecast> Post([FromForm]IFormFile file)
        {
            //记录多用户日志
            Log.ForContext("RequestId", Guid.NewGuid().ToString())
               .ForContext("UserId", Guid.NewGuid().ToString())
               .ForContext("LogType", Guid.NewGuid().ToString())
               .Information("This is a custom log message");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
