using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.Core.Countries.Query;
using UserManagement.Domain.Entities;

namespace UserManagement.Tests.Core.Countries.Query
{
    public class GetAllCountriesQueryHandlerTests
    {
        private readonly Mock<ICountryRepository> _countryRepositoryMock;
        private readonly Mock<IRedisCacheService> _redisCacheServiceMock;
        private readonly GetAllCountriesQueryHandler _handler;

        public GetAllCountriesQueryHandlerTests()
        {
            _countryRepositoryMock = new Mock<ICountryRepository>();
            _redisCacheServiceMock = new Mock<IRedisCacheService>();
            _handler = new GetAllCountriesQueryHandler(_countryRepositoryMock.Object, _redisCacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCountriesFromCache_WhenCacheExists()
        {
            // Arrange
            var userId = "12345";
            var query = new GetAllCountriesQuery { UserId = userId };
            var cachedCountries = new List<CountryDto>
            {
                new CountryDto(1, "US", "United States"),
                new CountryDto(2, "CA", "Canada"),
            };

            _redisCacheServiceMock.Setup(x => x.GetOrSetCacheValueAsync<IEnumerable<CountryDto>>(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<CountryDto>>>>()))
                .ReturnsAsync(cachedCountries);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("United States", result.First().Name);
            _countryRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnCountriesFromRepository_WhenCacheDoesNotExist()
        {
            // Arrange
            var userId = "12345";
            var query = new GetAllCountriesQuery { UserId = userId };
            var countriesFromDb = new List<Country>
            {
                new Country { Id = 1, Code = "US", Name = "United States" },
                new Country { Id = 2, Code = "CA", Name = "Canada" },
            };

            _redisCacheServiceMock.Setup(x => x.GetOrSetCacheValueAsync<IEnumerable<CountryDto>>(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<CountryDto>>>>()))
                .ReturnsAsync(() => null); // Simulate cache miss

            _countryRepositoryMock.Setup(x => x.GetAllAsync(query.PageNumber, query.PageSize))
                .Returns(countriesFromDb.AsQueryable()); // Return IQueryable

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("United States", result.First().Name);
            _countryRepositoryMock.Verify(x => x.GetAllAsync(query.PageNumber, query.PageSize), Times.Once);
        }
        
        // to do
        // public async Task Handle_ShouldThrowException_WhenRepositoryThrowsException()
        // {
        //     // Arrange
        //     var userId = "12345";
        //     var query = new GetAllCountriesQuery { UserId = userId };

        //     _redisCacheServiceMock.Setup(x => x.GetOrSetCacheValueAsync<IEnumerable<CountryDto>>(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<CountryDto>>>>()))
        //         .ReturnsAsync(() => null); // Simulate cache miss

        //     // Setup the mock to throw an exception when called
        //     _countryRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
        //         .ThrowsAsync(new System.Exception("Database error")); // Correctly set up the ThrowsAsync on the Task

        //     // Act & Assert
        //     await Assert.ThrowsAsync<System.Exception>(() => _handler.Handle(query, CancellationToken.None));
        // }
    }
}