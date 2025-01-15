using Microsoft.AspNetCore.Antiforgery;

namespace UserManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AntiForgeryController : ControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        [HttpGet("GetToken")]
        public IActionResult GetToken()
        {
            // Generate the token
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            var token = tokens.RequestToken;

            // Return the token in a response
            return Ok(new { Token = token });
        }

        // If you're using cookies for antiforgery, make sure you have an endpoint to set the token
        [HttpPost("ValidateToken")]
        public IActionResult ValidateToken()
        {
            try
            {
                // This will automatically check the token for POST requests
                _antiforgery.ValidateRequestAsync(HttpContext);
                return Ok();
            }
            catch
            {
                return Unauthorized("Invalid anti-forgery token.");
            }
        }
    }

