using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgEntLib.Service;

namespace ProgEntLib.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/categoria")]
    public class ControllerCategoria : ControllerBase
    {
        private readonly CategoriaService _categoriaService;
        private readonly ILogger<UtenteService> _logger;

        public ControllerCategoria(CategoriaService categoriaService, ILogger<UtenteService> logger)
        {
            _categoriaService = categoriaService ?? throw new ArgumentNullException(nameof(categoriaService));
            _logger = logger;
        }

        [HttpPost("crea")]
        public async Task<IActionResult> CreaCategoria(string nome)
        {
            var success = await _categoriaService.CreaCategoriaAsync(nome);
            
            if (success == null)
            {
                return BadRequest("Categoria gia esistente");
            }

            return Ok("Categoria "+ nome + " creata");
        }

        [HttpDelete("elimina/{nome}")]
        public async Task<IActionResult> CancellaCategoria(string nome)
        {
            var success = await _categoriaService.CancellaCategoriaAsync(nome);
            if (!success)
                return BadRequest("Categoria legata a dei libri, non è possibile eliminarla!!");

            return Ok("Categoria "+ nome +" cancellata");
        }

        [HttpGet("allCategorie")]
        public async Task<IActionResult> tutteCategorie()
        {
            var categorie = await _categoriaService.tutteCategorieAsync();
            return Ok(categorie);
        }
    }
}
