using System.ComponentModel.DataAnnotations;

namespace Batch.Models
{
    public enum EstadoBatch
    {
        PendienteDeLlenado = 0,
        LlenadoAprobado = 1,
        LlenadoRechazado = 2,
        Consumido = 3
    }

    public class Lote
    {
        public int Id { get; set; }
        public string Folio { get; set; }
        public string RegistroId { get; set; }
        public int ComponenteId { get; set; }
        public Componente Componente { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaExp { get; set; }

        public EstadoBatch Estado { get; set; } = EstadoBatch.PendienteDeLlenado;

        // Nueva columna
        public DateTime? FechaCambioEstado { get; set; }

        public ICollection<ResultadoPrueba> Resultados { get; set; }
    }
}