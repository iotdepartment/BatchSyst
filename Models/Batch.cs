using System.ComponentModel.DataAnnotations;

namespace Batch.Models
{
    public class Lote
    {
        public int Id { get; set; }
        public string Folio { get; set; }
        public int ComponenteId { get; set; }
        public Componente Componente { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaExp { get; set; }
        public string RegistroId { get; set; }

        public ICollection<ResultadoPrueba> Resultados { get; set; }
    }
}