using Batch.Data;

namespace Batch.Helper
{
    public static class FolioHelper
    {
        public static string CrearFolio(DateTime ahora, int turno, string linea, int componenteId, AppDbContext context, bool retrabajo = false)
        {
            DateTime fechaFolio;

            // ⏰ Entre 00:00 y 06:59 seguimos en el día laboral anterior
            if (ahora.TimeOfDay < new TimeSpan(7, 0, 0))
            {
                if (turno == 3)
                {
                    // ✅ Turno 3 → pertenece al día actual
                    fechaFolio = ahora.Date;
                }
                else
                {
                    // ✅ Turno 1 o 2 → pertenecen al día laboral anterior
                    fechaFolio = ahora.Date.AddDays(-1);
                }
            }
            else
            {
                // ✅ De 07:00 a 23:59 → todos los turnos pertenecen al día actual
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

            // ✅ Prefijo del folio
            string prefijo = $"{yearLetter}{monthLetter}{day}{lineaLetter}";

            // ✅ Contar folios existentes con ese prefijo
            var totalHoy = context.Batches
                .Count(b => b.ComponenteId == componenteId &&
                            b.Folio.StartsWith(prefijo)) + 1;

            string consecutivo = totalHoy.ToString("00");

            // ✅ Folio final
            var folio = prefijo + consecutivo;

            if (retrabajo)
                folio = "R" + folio;

            return folio;
        }
    }
}