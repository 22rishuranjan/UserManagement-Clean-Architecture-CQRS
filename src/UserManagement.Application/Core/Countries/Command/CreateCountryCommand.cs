
using UserManagement.Application.Common.Interfaces;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Core.Countries.Command
{
    public record CreateCountryCommand : IRequest<int>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        private const string RedisNamespace = "UserManagement:Country";

        public string GetRedisKey()
        {
            return $"{RedisNamespace}:all";
        }
    }

    public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, int>
    {
        private readonly ICountryRepository _repository;
        private readonly IRedisCacheService _redisCacheService;

        public CreateCountryCommandHandler(ICountryRepository repository, IRedisCacheService redisCacheService)
        {
            _repository = repository;
            _redisCacheService = redisCacheService;
        }

        public async Task<int> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = new Country
            {
                Code = request.Code,
                Name = request.Name
            };
           
            
            var result = await _repository.CreateAsync(country, cancellationToken);

            // Clear cache
            string cacheKey = request.GetRedisKey();
            await _redisCacheService.RemoveCacheAsync(cacheKey);

            return result.Id; 
        }
    }



}
