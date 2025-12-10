namespace Batch.Models
{
    public class Permiso
    {
        public int Id { get; set; }

        public string Nombre { get; set; } // Ej: "CrearBatch", "EvaluarBatch"

        public ICollection<RolPermiso> RolPermisos { get; set; }
    }
}
