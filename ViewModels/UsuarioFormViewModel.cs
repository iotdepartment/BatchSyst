using System.Collections.Generic;
using Batch.Models; // si tus roles están en Models

namespace Batch.ViewModels
{
    public class UsuarioFormViewModel
    {
        public int Id { get; set; } = 0; // 0 = crear, >0 = editar

        public string Nombre { get; set; }
        public string UsuarioLogin { get; set; }
        public string Password { get; set; }

        public int RolId { get; set; }

        public List<Rol> RolesDisponibles { get; set; } = new List<Rol>();
    }
}