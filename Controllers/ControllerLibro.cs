using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProgEntLib.DTO;

namespace ProgEntLib.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ControllerLibro
    {
        private readonly LibroService _libroService;

        public ControllerLibro(LibroService libroService)
        {
            _libroService = libroService;
        }

        [HttpGet]
        public async Task<IActionResult> CreaLibro([FromBody] DTOLibro newBook)
        {
            if (!ModelState.IsValid) {
                return BadReques
        }
    }
}
