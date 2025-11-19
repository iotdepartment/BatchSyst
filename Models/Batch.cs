using System.ComponentModel.DataAnnotations;

namespace Batch.Models
{
    public class Lote
    {
        public int Id { get; set; }
        [Required, StringLength(30)]
        public string Folio { get; set; }
        [Required]
        public int ComponenteId { get; set; }
        public Componente Componente { get; set; }
        [Required]
        public DateTime FechaInicio { get; set; }
        [Required]
        public DateTime FechaExp { get; set; }
        public ICollection<ResultadoPrueba> Resultados { get; set; }
    }
}