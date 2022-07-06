using Microsoft.AspNetCore.Mvc;
using SimpleApp.Interfaces;
using SimpleApp.Data.Entities;

namespace SimpleApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastRepository _weatherForecastRepository;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastRepository weatherForecastRepository)
        {
            _logger = logger;
            _weatherForecastRepository = weatherForecastRepository;
        }

        [Route("{date}")]
        public WeatherForecast? Get(DateTime date)
        {
            return _weatherForecastRepository.Get(date);
        }

        [HttpPost]
        public WeatherForecast InsertOrUpdate([FromBody] WeatherForecast weatherForecast)
        {
            return _weatherForecastRepository.InsertOrUpdate(weatherForecast);
        }
    }
}