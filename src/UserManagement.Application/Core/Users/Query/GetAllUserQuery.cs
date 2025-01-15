
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Application.Core.Users.Query;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
{
    public int PageNumber { get; set; } = 1; // Default to first page
    public int PageSize { get; set; } = 10; // Default page size

    public string UserId { get; set; } = string.Empty; // Unique identifier for the user

    // Redis namespace for this query
    private const string RedisNamespace = "UserManagement:GetAllUsers";


    // Method to generate the Redis key
    public string GetRedisKey()
    {
        return $"{RedisNamespace}:user-{UserId}:page-{PageNumber}:size-{PageSize}";
    }
}


    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly IRedisCacheService _redisCacheService;

    public GetAllUsersQueryHandler(IUserRepository repository, IRedisCacheService redisCacheService)
    {
        _repository = repository;
        _redisCacheService = redisCacheService;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = request.GetRedisKey();
        return await _redisCacheService.GetOrSetCacheValueAsync<IEnumerable<UserDto>>(
            cacheKey, () => GetAllUsers(request.PageNumber, request.PageSize, cancellationToken)
            );
    }

    private async Task<IEnumerable<UserDto>> GetAllUsers(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var users = _repository.GetAllAsync(pageNumber, pageSize);
       
        return await users.Select(user => 
                new UserDto(user.Id, user.FirstName, user.LastName, user.Email, user.DateOfBirth, user.CountryId)
        ).ToListAsync(cancellationToken);
      
    }
}

