using System;

namespace Batch.ViewModels
{
    public class UsuarioItemViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UsuarioLogin { get; set; }
        public string RolNombre { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}