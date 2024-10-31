using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgEntLib.DTO;
using ProgEntLib.Service;

namespace ProgEntLib.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[utente]")]
    public class ControllerUtente : ControllerBase
    {
        private readonly UtenteService _utenteService;

        public ControllerUtente(UtenteService _utenteService)
        {
            _utenteService = this._utenteService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTOUtente utente)
        {
            var success = await _utenteService.CreaUtenteAsync(utente);
            if (success == null)
            {
                return BadRequest("Nessun nuovo utente registrato!!");
            }

            return Ok(success);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTOLogin login)
        {
            var token = await _utenteService.AutenticaUtenteAsync(login);
            if (token == null)
            {
                return Unauthorized("Invalid email or password!!");
            }

            return Ok(new { Token = token });
        }
    }
}
