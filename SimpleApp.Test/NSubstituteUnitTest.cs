using FluentAssertions;
using SimpleApp.Data.Entities;
using SimpleApp.Data.Repositories;
using SimpleApp.Data;
using NSubstitute;
using Microsoft.EntityFrameworkCore;

namespace SimpleApp.Test
{
    public class NSubstituteUnitTest
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private WeatherForecastRepository _weatherForecastRepository;
        private AppDbContext _appDbContext;

        [SetUp]
        public void Setup()
        {
            var queryableList = new List<WeatherForecast>()
                {
                    new WeatherForecast()
                    {
                        Id = 1,
                        Date = new DateTime(2022,7,2),
                        TemperatureC = 25,
                        Summary = Summaries[8]
                    },
                    new WeatherForecast()
                    {
                        Id = 2,
                        Date = new DateTime(2022,7,3),
                        TemperatureC = 35,
                        Summary = Summaries[8]
                    },
                    new WeatherForecast()
                    {
                        Id = 3,
                        Date = new DateTime(2022,7,4),
                        TemperatureC = 32,
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    }
                }.AsQueryable();

            var mockSet = Substitute.For<DbSet<WeatherForecast>, IQueryable<WeatherForecast>>();
            ((IQueryable<WeatherForecast>)mockSet).Provider.Returns(queryableList.Provider);
            ((IQueryable<WeatherForecast>)mockSet).Expression.Returns(queryableList.Expression);
            ((IQueryable<WeatherForecast>)mockSet).ElementType.Returns(queryableList.ElementType);
            ((IQueryable<WeatherForecast>)mockSet).GetEnumerator().Returns(queryableList.GetEnumerator());

            _appDbContext = Substitute.For<AppDbContext>();
            _appDbContext.WeatherForecast.Returns(mockSet);

            _weatherForecastRepository = new WeatherForecastRepository(_appDbContext);
        }

        [Test]
        public void GetByDate_ReturnNull()
        {
            var result = _weatherForecastRepository.Get(new DateTime(2022, 7, 1));
            result.Should().BeNull();
        }

        [Test]
        public void GetByDate_ReturnSuccess()
        {
            var result = _weatherForecastRepository.Get(new DateTime(2022, 7, 2));
            result?.Id.Should().Be(1);
        }

        [Test]
        public void GetByDates_ReturnSet()
        {
            var result = _weatherForecastRepository.Get(new DateTime(2022, 7, 3), new DateTime(2022, 7, 4));
            result.Count().Should().Be(2);
            result.Any(x => x.Id == 2).Should().BeTrue();
        }

        [Test]
        public void InsertOrUpdate_UpdateReturnException()
        {            
            Assert.That(() => _weatherForecastRepository.InsertOrUpdate(new WeatherForecast()
            {
                Id = 5,
                Date = new DateTime(2022, 7, 3),
                TemperatureC = 135,
                Summary = Summaries[8]
            }),
            Throws.Exception.TypeOf<Exception>());
        }

        [Test]
        public void InsertOrUpdate_UpdateSuccess()
        {
            var result = _weatherForecastRepository.InsertOrUpdate(new WeatherForecast()
            {
                Id = 2,
                Date = new DateTime(2022, 7, 3),
                TemperatureC = 20,
                Summary = String.Empty
            });

            result.Id.Should().Be(2);
            _appDbContext.Received(1).SaveChanges();
        }

        [Test]
        public void InsertOrUpdate_InsertSuccess()
        {
            var result = _weatherForecastRepository.InsertOrUpdate(new WeatherForecast()
            {
                Date = new DateTime(2022, 7, 10),
                TemperatureC = 20,
                Summary = String.Empty
            });

            result.Date.Day.Should().Be(10);
            _appDbContext.Received(1).SaveChanges();
        }

        [Test]
        public void AverageTemperature_Success() 
        { 
            var result = _weatherForecastRepository.AverageTemperature(new DateTime(2022, 7, 3), new DateTime(2022, 7, 4));
            result.Should().Be(33.5); 
            result.Should().BeGreaterThan(30);
        }
    }
}