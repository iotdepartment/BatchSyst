using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Batch.Models
{
    public enum EstadoBatch
    {
        PendienteDeLlenado = 0,
        LlenadoAprobado = 1,
        LlenadoRechazado = 2,
        RFIDAsignado = 3,
        Consumido = 4
    }

    public class Lote
    {
        public int Id { get; set; }

        public string Folio { get; set; }

        public string RegistroId { get; set; }

        public int ComponenteId { get; set; }

        public Componente Componente { get; set; }

        public int Turno { get; set; } // 1, 2 o 3

        public DateTime FechaInicio { get; set; }

        public DateTime FechaExp { get; set; }

        public EstadoBatch Estado { get; set; } = EstadoBatch.PendienteDeLlenado;

        public DateTime? FechaCambioEstado { get; set; }

        public string? RFID { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaCreacion { get; set; }

        public ICollection<ResultadoPrueba> Resultados { get; set; }

        // ✅ NUEVO: Usuario que creó el batch
        public string UsuarioId { get; set; }

        public Usuario Usuario { get; set; }

        public Lote()
        {
            FechaCreacion = DateTime.Now;
        }
    }
}