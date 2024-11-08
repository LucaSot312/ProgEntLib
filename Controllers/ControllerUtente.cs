using Microsoft.AspNetCore.Mvc;
using ProgEntLib.DTO;
using ProgEntLib.Service;

namespace ProgEntLib.Controllers
{
    [ApiController]
    [Route("api/utente")]
    public class ControllerUtente : ControllerBase
    {
        private readonly UtenteService _utenteService;

        public ControllerUtente(UtenteService utenteService)
        {
            _utenteService = utenteService ?? throw new ArgumentNullException(nameof(utenteService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTOUtente utente)
        {
            if (utente == null)
            {
                return BadRequest("Dati utente non validi.");
            }

            if (string.IsNullOrEmpty(utente.Nome) || string.IsNullOrEmpty(utente.Email) || string.IsNullOrEmpty(utente.Password))
            {
                return BadRequest("Tutti i campi sono obbligatori.");
            }

            var result = await _utenteService.CreaUtenteAsync(utente);

            if (result == null)
            {
                return Conflict("L'utente esiste già.");
            }

            return Ok("Registrazione avvenuta con successo.");
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
