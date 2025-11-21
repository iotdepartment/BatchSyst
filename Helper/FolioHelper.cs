using Batch.Data;

namespace Batch.Helper
{
    public static class FolioHelper
    {
        public static string CrearFolio(DateTime fecha, string linea, int componenteId, AppDbContext context, bool retrabajo = false)
        {
            // Año → letra automática (2025 = V)
            char yearLetter = (char)('V' + (fecha.Year - 2025));

            // Mes → letra automática (Enero = A, Diciembre = L)
            char monthLetter = (char)('A' + (fecha.Month - 1));

            // Día → número con dos dígitos
            string day = fecha.Day.ToString("00");

            // Línea → letra (A, B, C)
            string lineaLetter = linea switch
            {
                "1" => "A",
                "2" => "B",
                "3" => "C",
                _ => "X"
            };

            // Número de batch por componente en ese día
            var totalHoy = context.Batches
                .Count(b => b.ComponenteId == componenteId && b.FechaInicio.Date == fecha.Date) + 1;

            string consecutivo = totalHoy.ToString("00");

            // Folio final
            var folio = $"{yearLetter}{monthLetter}{day}{lineaLetter}{consecutivo}";

            // Si es retrabajo, anteponer R
            if (retrabajo)
                folio = "R" + folio;

            return folio;
        }
    }
}
