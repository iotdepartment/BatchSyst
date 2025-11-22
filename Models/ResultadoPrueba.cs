using System.ComponentModel.DataAnnotations;

namespace Batch.Models
{
    public class ResultadoPrueba
    {
        public int Id { get; set; }
        public int LoteId { get; set; }
        public Lote Batch { get; set; }

        public int ToleranciaId { get; set; }
        public Tolerancia Tolerancia { get; set; }

        // Cambiar de float/double a nullable
        public float? Valor { get; set; }

        public bool EsValido { get; set; }
    }
}