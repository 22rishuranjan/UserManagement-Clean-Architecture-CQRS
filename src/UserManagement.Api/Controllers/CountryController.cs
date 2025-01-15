using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.Core.Countries.Command;
using UserManagement.Application.Core.Countries.Query;
using UserManagement.Application.Core.Users.Query;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CountryController : BaseApiController
{
    private readonly IMediator _mediator;

    private readonly ILogger<CountryController> _logger;

    public CountryController(ILogger<CountryController> logger, IMediator mediator)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCountry([FromBody] CreateCountryCommand command)
    {
        var countryId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAllCountries), new { id = countryId }, null);
    }

    [HttpGet]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountries()
    {
        // Use Mediator if you wish to encapsulate the query as well
        var countries = await _mediator.Send(new GetAllCountriesQuery());
        return Ok(countries);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryCommand command)
    {
        await _mediator.Send(command);
        _logger.LogInformation("Country with ID {Id} updated successfully.", id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
       
        await _mediator.Send(new DeleteCountryCommand { Id = id });
        _logger.LogInformation("Country with ID {Id} deleted successfully.", id);

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CountryDto>> GetCountryById(int id)
    {
        var country = await _mediator.Send(new GetCountryByIdQuery { Id = id });
        if (country == null)
        {
            _logger.LogWarning("Country with ID {Id} not found.", id);
            return NotFound($"Country with ID {id} not found.");
        }

        _logger.LogInformation("Country with ID {Id} retrieved successfully.", id);
        return Ok(country);
    }

}

