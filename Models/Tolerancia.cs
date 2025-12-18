using System.ComponentModel.DataAnnotations;

namespace Batch.Models
{
    public class Tolerancia
    {
        public int Id { get; set; }
        public string Prueba { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }

        public int ComponenteId { get; set; }
        public Componente Componente { get; set; }
    }

}
