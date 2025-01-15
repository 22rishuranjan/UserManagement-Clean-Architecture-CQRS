
using UserManagement.Application.Core.Users.Command;
using UserManagement.Application.Core.Users.Query;

namespace UserManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseApiController
{
    private readonly IMediator _mediator;

    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger, IMediator mediator)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUserById), new { id = userId }, null);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
        if (user == null)
        {
            _logger.LogWarning("User with ID {Id} not found.", id);
            return NotFound($"User with ID {id} not found.");
        }

        _logger.LogInformation("User with ID {Id} retrieved successfully.", id);
        return Ok(user);
    }

    [HttpGet]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());
        _logger.LogInformation("All users retrieved successfully.");

        return Ok(users);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserCommand command)
    {
       
        await _mediator.Send(command);
       
        return NoContent();
    }

  
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        
        await _mediator.Send(new DeleteUserCommand { Id = id });
        
        return NoContent();
    }

}

