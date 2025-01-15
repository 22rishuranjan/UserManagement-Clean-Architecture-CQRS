using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Application.Core.Countries.Command
{
    public record UpdateCountryCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        private const string RedisNamespace = "UserManagement:Country";

        public string GetRedisKey()
        {
            return $"{RedisNamespace}:all";
        }
    }

    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, Unit>
    {
        private readonly ICountryRepository _repository;
        private readonly IRedisCacheService _redisCacheService;

        public UpdateCountryCommandHandler(ICountryRepository repository, IRedisCacheService redisCacheService)
        {
            _repository = repository;
            _redisCacheService = redisCacheService;
        }

        public async Task<Unit> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (country == null)
            {
                throw new KeyNotFoundException($"Country with ID {request.Id} not found.");
            }

            country.Code = request.Code;
            country.Name = request.Name;

            await _repository.UpdateAsync(country, cancellationToken);

            // Clear cache
            string cacheKey = request.GetRedisKey();
            await _redisCacheService.RemoveCacheAsync(cacheKey);

            return Unit.Value;
        }

     
    }


}
