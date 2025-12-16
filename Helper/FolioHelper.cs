using Batch.Data;

namespace Batch.Helper
{
    public static class FolioHelper
    {
        public static string CrearFolio(DateTime ahora, int turno, string linea, int componenteId, AppDbContext context, bool retrabajo = false)
        {
            // ✅ Determinar fecha del folio según tu lógica
            DateTime fechaFolio;

            // ⏰ Entre 00:00 y 06:59
            if (ahora.TimeOfDay < new TimeSpan(7, 0, 0))
            {
                if (turno == 2)
                    fechaFolio = ahora.Date.AddDays(-1); // turno 2 → ayer
                else
                    fechaFolio = ahora.Date;            // turno 3 → hoy
            }
            else
            {
                // ✅ Horario normal → hoy
                fechaFolio = ahora.Date;
            }

            // ✅ Año → letra automática (2025 = V)
            char yearLetter = (char)('V' + (fechaFolio.Year - 2025));

            // ✅ Mes → letra automática (Enero = A, Diciembre = L)
            char monthLetter = (char)('A' + (fechaFolio.Month - 1));

            // ✅ Día → número con dos dígitos
            string day = fechaFolio.Day.ToString("00");

            // ✅ Línea → letra (A, B, C)
            string lineaLetter = linea switch
            {
                "1" => "A",
                "2" => "B",
                "3" => "C",
                _ => "X"
            };

            // ✅ Prefijo del folio (sin consecutivo)
            string prefijo = $"{yearLetter}{monthLetter}{day}{lineaLetter}";

            // ✅ Contar folios existentes con ese prefijo
            var totalHoy = context.Batches
                .Count(b => b.ComponenteId == componenteId &&
                            b.Folio.StartsWith(prefijo)) + 1;

            string consecutivo = totalHoy.ToString("00");

            // ✅ Folio final
            var folio = prefijo + consecutivo;

            // ✅ Si es retrabajo, anteponer R
            if (retrabajo)
                folio = "R" + folio;

            return folio;
        }
    }
}