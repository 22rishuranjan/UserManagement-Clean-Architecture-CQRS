
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Application.Core.Countries.Command
{
    public record DeleteCountryCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        private const string RedisNamespace = "UserManagement:Country";

        public string GetRedisKey()
        {
            return $"{RedisNamespace}:all";
        }
    }

    public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, Unit>
    {
        private readonly ICountryRepository _repository;
        private readonly IRedisCacheService _redisCacheService;

        public DeleteCountryCommandHandler(ICountryRepository repository, IRedisCacheService redisCacheService)
        {
            _repository = repository;
            _redisCacheService = redisCacheService;
        }

        public async Task<Unit> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (country == null)
            {
                throw new KeyNotFoundException($"Country with ID {request.Id} not found.");
            }

            await _repository.DeleteAsync(country.Id, cancellationToken);

            // Clear cache
            string cacheKey = request.GetRedisKey();
            await _redisCacheService.RemoveCacheAsync(cacheKey);

            return Unit.Value;
        }
    }


}
