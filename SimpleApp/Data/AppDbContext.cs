using Microsoft.EntityFrameworkCore;
using SimpleApp.Data.Entities;

namespace SimpleApp.Data
{
    public class AppDbContext : DbContext
    {

        public virtual DbSet<WeatherForecast> WeatherForecast { get; set; }    
    }
}
