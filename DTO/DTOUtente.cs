using System.ComponentModel.DataAnnotations;

namespace ProgEntLib.DTO
{
    public class DTOUtente
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public string Cognome { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }

    }
}
