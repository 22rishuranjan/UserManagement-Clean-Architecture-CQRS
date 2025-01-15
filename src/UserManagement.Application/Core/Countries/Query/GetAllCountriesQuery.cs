
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Application.Core.Countries.Query;
public record GetAllCountriesQuery : IRequest<IEnumerable<CountryDto>>
{
    public int PageNumber { get; set; } = 1; // Default to first page
    public int PageSize { get; set; } = 10; // Default page size

    public string UserId { get; set; } = string.Empty; // Unique identifier for the user


    // Redis namespace for this query
    private const string RedisNamespace = "UserManagement:GetAllCountries";


    // Method to generate the Redis key
    public string GetRedisKey()
    {
        return $"{RedisNamespace}:user-{UserId}:page-{PageNumber}:size-{PageSize}";
    }
}

public class GetAllCountriesQueryHandler : IRequestHandler<GetAllCountriesQuery, IEnumerable<CountryDto>>
{
    private readonly ICountryRepository _repository;
    private readonly IRedisCacheService _redisCacheService;
    public GetAllCountriesQueryHandler(ICountryRepository repository, IRedisCacheService redisCacheServicedis)
    {
        _repository = repository;
        _redisCacheService = redisCacheServicedis;
    }

    public async Task<IEnumerable<CountryDto>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = request.GetRedisKey();

        return await _redisCacheService.GetOrSetCacheValueAsync<IEnumerable<CountryDto>>(
             cacheKey, () => GetAllCountries(request.PageNumber, request.PageSize, cancellationToken)
             );
    }

    private async Task<IEnumerable<CountryDto>> GetAllCountries(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var countries =  _repository.GetAllAsync(pageNumber, pageSize);
        var data = await  countries
            .Select(country => new CountryDto(country.Id, country.Code, country.Name))
            .ToListAsync(cancellationToken);
        return data;
    }
}
