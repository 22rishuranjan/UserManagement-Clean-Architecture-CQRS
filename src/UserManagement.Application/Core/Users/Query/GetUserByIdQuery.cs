
using UserManagement.Application.Common.Interfaces;

namespace UserManagement.Application.Core.Users.Query
{
    public record GetUserByIdQuery : IRequest<UserDto>
    {
        public int Id { get; set; }
    }


    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.Id} not found.");
            }

            return new UserDto(user.Id, user.FirstName, user.LastName, user.Email, user.DateOfBirth, user.CountryId);
            
        }
    }

}
