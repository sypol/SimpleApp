using SimpleApp.Data.Entities;

namespace SimpleApp.Interfaces
{
    public interface IWeatherForecastRepository
    {
        WeatherForecast? Get(DateTime date);
        List<WeatherForecast> Get(DateTime dateFrom, DateTime dateTo);
        WeatherForecast InsertOrUpdate(WeatherForecast weatherForecast);
        void Remove(WeatherForecast weatherForecast);
        double AverageTemperature(DateTime dateFrom, DateTime dateTo);
    }
}
