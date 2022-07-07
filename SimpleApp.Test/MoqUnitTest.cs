using Moq;
using FluentAssertions;
using SimpleApp.Data.Entities;
using SimpleApp.Data.Repositories;
using SimpleApp.Data;
using Microsoft.EntityFrameworkCore;

namespace SimpleApp.Test
{
    public class MoqUnitTest
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private Mock<AppDbContext> _appDbContext;
        private Mock<DbSet<WeatherForecast>> _mockSet;
        private WeatherForecastRepository _weatherForecastRepository;

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

            _mockSet = new Mock<DbSet<WeatherForecast>>();
            _mockSet.As<IQueryable<WeatherForecast>>().Setup(m => m.Provider).Returns(queryableList.Provider);
            _mockSet.As<IQueryable<WeatherForecast>>().Setup(m => m.Expression).Returns(queryableList.Expression);
            _mockSet.As<IQueryable<WeatherForecast>>().Setup(m => m.ElementType).Returns(queryableList.ElementType);
            _mockSet.As<IQueryable<WeatherForecast>>().Setup(m => m.GetEnumerator()).Returns(queryableList.GetEnumerator());

            _appDbContext = new Mock<AppDbContext>();
            _appDbContext.Setup(x => x.WeatherForecast).Returns(_mockSet.Object);

            _weatherForecastRepository = new WeatherForecastRepository(_appDbContext.Object);
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
            _weatherForecastRepository.InsertOrUpdate(new WeatherForecast()
            {
                Id = 2,
                Date = new DateTime(2022, 7, 3),
                TemperatureC = 20,
                Summary = String.Empty
            });

            _mockSet.Verify(m => m.Update(It.IsAny<WeatherForecast>()), Times.Once());
            _mockSet.Verify(m => m.Add(It.IsAny<WeatherForecast>()), Times.Never());
            _appDbContext.Verify(x => x.SaveChanges(), Times.Once());
        }

        [Test]
        public void InsertOrUpdate_InsertSuccess()
        {
            _weatherForecastRepository.InsertOrUpdate(new WeatherForecast()
            {
                Date = new DateTime(2022, 7, 10),
                TemperatureC = 20,
                Summary = String.Empty
            });

            _mockSet.Verify(m => m.Add(It.IsAny<WeatherForecast>()), Times.Once());
            _appDbContext.Verify(x => x.SaveChanges(), Times.Once());
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