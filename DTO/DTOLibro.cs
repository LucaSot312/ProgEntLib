using System.ComponentModel.DataAnnotations;
using ProgEntLib.Models;

namespace ProgEntLib.DTO
{
    public class DTOLibro
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public string Autore { get; set; }

        [Required]
        public DateOnly DataPubblicazione { get; set; }

        [Required]
        public string Editore { get; set; }

        [Required]
        public List<DTOCategoria> Categorie { get; set; }
    }
}
