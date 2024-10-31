using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgEntLib.DTO;
using ProgEntLib.Service;

namespace ProgEntLib.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[categoria]")]
    public class ControllerCategoria : ControllerBase
    {
        private readonly CategoriaService _categoriaService;

        public ControllerCategoria(CategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpPost]
        public async Task<IActionResult> CreaCategoria([FromBody] DTOCategoria categoria)
        {
            var success = await _categoriaService.CreaCategoriaAsync(categoria);
            if (success.Equals(null))
            {
                return BadRequest("Categoria già esistente!!");
            }

            return Ok(success);
        }

        [HttpDelete("{nome}")]
        public async Task<IActionResult> CancellaCategoria(string nome)
        {
            var success = await _categoriaService.CancellaCategoriaAsync(nome);
            if (!success)
                return BadRequest("Categoria legata a dei libri, non è possibile eliminarla!!");

            return Ok("Categoria "+ nome +" cancellata");
        }

        [HttpGet]
        public async Task<IActionResult> tutteCategorie()
        {
            var categorie = await _categoriaService.tutteCategorieAsync();
            return Ok(categorie);
        }
    }
}
