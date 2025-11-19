namespace Batch.Models
{
    public class Componente
    {
        public int Id { get; set; }          // PK
        public string Name { get; set; }     // Nombre del componente
        public int ExpDays { get; set; }     // Días de expiración
        public string BatchCar { get; set; } // Número/lote asignado
    }
}
