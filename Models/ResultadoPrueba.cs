using System.ComponentModel.DataAnnotations;

namespace Batch.Models
{
    public class ResultadoPrueba
    {
        public int Id { get; set; }
        [Required]
        public int BatchId { get; set; }
        public Lote Batch { get; set; } // Tipo de navegación ahora es Lote
        [Required]
        public int ToleranciaId { get; set; }
        public Tolerancia Tolerancia { get; set; }
        public float Valor { get; set; }
    }
}