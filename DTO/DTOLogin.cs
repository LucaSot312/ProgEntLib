using System.ComponentModel.DataAnnotations;

namespace ProgEntLib.DTO
{
    public class DTOLogin
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
