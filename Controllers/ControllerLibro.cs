using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgEntLib.DTO;
using ProgEntLib.Service;

namespace ProgEntLib.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/libro")]
    public class ControllerLibro : ControllerBase
    {
        private readonly LibroService _libroService;

        public ControllerLibro(LibroService libroService)
        {
            _libroService = libroService;
        }

        [HttpPost("crea")]
        public async Task<IActionResult> CreaLibro([FromBody] DTOLibro newBook)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookId = await _libroService.CreaLibroAsync(newBook);
            return CreatedAtAction(nameof(GetLibroById), new { id = bookId }, newBook);
        }

        [HttpGet("libroById/{id}")]
        public async Task<IActionResult> GetLibroById(string id)
        {
            var book = await _libroService.GetLibroByIdAsync(id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpPut("aggiorna/{id}")]
        public async Task<IActionResult> UpdateBook(string id, [FromBody] DTOLibro dtoLibro)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _libroService.AggiornaLibroAsync(id, dtoLibro);
            if (!result)
                return NotFound();

            return Ok("Libro aggiornato");
        }

        [HttpDelete("elimina/{id}")]
        public async Task<IActionResult> CancellaLibro(string id)
        {
            var result = await _libroService.CancellaLibroAsync(id);
            if (!result)
                return NotFound();

            return Ok("Libro rimosso");
        }

        [HttpGet("search")]
        public async Task<IActionResult> CercaLibri(
            [FromQuery] string? categoria = null,
            [FromQuery] string? nome = null,
            [FromQuery] string? dataPubblicazione = null,
            [FromQuery] string? autore = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            if (page < 1 || pageSize < 1)
                return BadRequest("Pagine e il loro contenuto devono essere maggiori di 1");

            var result = await _libroService.CercaLibriAsync(categoria, 
                                                        nome, 
                                                        dataPubblicazione, 
                                                        autore, 
                                                        page, 
                                                        pageSize
                                                        );
            if (result == null)
                return BadRequest("Invalid date format. Use yyyy-MM-dd");

            return Ok(result);
        }
    }
}
