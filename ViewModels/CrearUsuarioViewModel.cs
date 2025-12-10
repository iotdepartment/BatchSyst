using System.ComponentModel.DataAnnotations;
using Batch.Models;

namespace Batch.ViewModels
{
    public class CrearUsuarioViewModel
    {
        
        public string Nombre { get; set; }
        public string UsuarioLogin { get; set; }
        public string Password { get; set; }
        public int RolId { get; set; }
        public List<Rol> RolesDisponibles { get; set; }
    }
}