using System.ComponentModel.DataAnnotations;

namespace Batch.Models
{
    public class Tolerancia
    {
        public int Id { get; set; }

        [Required]   // <- aquí sí, para obligar a elegir un componente
        public int ComponenteId { get; set; }

        public Componente? Componente { get; set; } // <- hazla nullable para evitar validación automática

        [Required]
        public string Prueba { get; set; }

        public float Max { get; set; }
        public float Min { get; set; }
    }

}
