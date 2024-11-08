using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgEntLib.Service;

namespace ProgEntLib.Controllers
{
    [Authorize]
    [Route("api/database")]
    [ApiController]
    public class ControllerDatabase : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public ControllerDatabase(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        
        // Endpoint per effettuare il backup del database
        [HttpPost("backup")]
        public async Task<IActionResult> BackupDatabase()
        {
            try
            {
                await _databaseService.BackupDatabaseAsync();
                return Ok("Backup completato con successo.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante il backup: {ex.Message}");
            }
        }

        // Endpoint per effettuare il restore del database
        [HttpPost("restore")]
        public async Task<IActionResult> RestoreDatabase([FromQuery] string backupFileName)
        {
            if (string.IsNullOrWhiteSpace(backupFileName))
            {
                return BadRequest("Specificare il nome del file di backup.");
            }

            try
            {
                await _databaseService.RestoreDatabaseAsync(backupFileName);
                return Ok("Restore completato con successo.");
            }
            catch (FileNotFoundException)
            {
                return NotFound("File di backup non trovato.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Errore durante il restore: {ex.Message}");
            }
        }
    }
}