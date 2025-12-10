namespace Batch.Models
{
    public class Rol
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public ICollection<UsuarioRol> UsuarioRoles { get; set; }
        public ICollection<RolPermiso> RolPermisos { get; set; }
    }
}
