using SimpleApp.Data.Entities;
using SimpleApp.Interfaces;

namespace SimpleApp.Data.Repositories
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private readonly AppDbContext _ctx;

        public WeatherForecastRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public void Remove(WeatherForecast weatherForecast)
        {
            _ctx.WeatherForecast.Remove(weatherForecast);
            _ctx.SaveChanges();
        }

        public WeatherForecast InsertOrUpdate(WeatherForecast weatherForecast)
        {
            _validate(weatherForecast);

            if (weatherForecast.Id > 0)
                _ctx.WeatherForecast.Update(weatherForecast);
            else
                _ctx.WeatherForecast.Add(weatherForecast);
            _ctx.SaveChanges();
            return weatherForecast;
        }

        public WeatherForecast? Get(DateTime date)
        {
            return _ctx.WeatherForecast.FirstOrDefault(x => x.Date == date);
        }

        public List<WeatherForecast> Get(DateTime dateFrom, DateTime dateTo)
        {
            return _ctx.WeatherForecast.Where(x => x.Date >= dateFrom && x.Date <= dateTo).ToList();
        }

        private void _validate(WeatherForecast weatherForecast) 
        {
            if (weatherForecast.TemperatureC > 100)
                throw new Exception("Temperature is too high");
            if (weatherForecast.TemperatureC < -100)
                throw new Exception("Temperature is too low");
        }

        public double AverageTemperature(DateTime dateFrom, DateTime dateTo)
        {
            return Math.Round(_ctx.WeatherForecast.Where(x => x.Date >= dateFrom && x.Date <= dateTo).Average(x => x.TemperatureC), 1);
        }
    }
}
