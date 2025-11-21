namespace Batch.Models
{
    public class Componente
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BatchCar { get; set; }

        public ICollection<Tolerancia> Tolerancias { get; set; }
    }
}
