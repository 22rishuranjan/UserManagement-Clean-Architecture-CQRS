
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Application.Core.Countries.Query
{
    public record GetCountryByIdQuery : IRequest<CountryDto>
    {
        public int Id { get; set; }

        private const string RedisNamespace = "UserManagement:Country";

        public string GetRedisKey()
        {
            return $"{RedisNamespace}:id-{Id}";
        }
    }

    public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, CountryDto>
    {
        private readonly ICountryRepository _repository;
        private readonly IRedisCacheService _redisCacheService;

        public GetCountryByIdQueryHandler(ICountryRepository repository, IRedisCacheService redisCacheService)
        {
            _repository = repository;
            _redisCacheService = redisCacheService;
        }

        public async Task<CountryDto> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = request.GetRedisKey();

            return await _redisCacheService.GetOrSetCacheValueAsync<CountryDto>(
                cacheKey,
                async () =>
                {
                    var country = await _repository.GetByIdAsync(request.Id, cancellationToken);
                    if (country == null)
                    {
                        throw new KeyNotFoundException($"Country with ID {request.Id} not found.");
                    }

                    return new CountryDto(country.Id, country.Code, country.Name);
              
                }
            );
        }
    }


}
