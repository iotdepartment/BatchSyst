using Microsoft.AspNetCore.Mvc.Rendering;

namespace Batch.ViewModels
{
    public class NuevoBatchModalViewModel
    {
        public string? RegistroId { get; set; }
        public int? ComponenteId { get; set; }
        public IEnumerable<SelectListItem> Componentes { get; set; } = Enumerable.Empty<SelectListItem>();
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaExp { get; set; }
    }
}
