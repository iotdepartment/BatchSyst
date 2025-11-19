using System;

namespace Batch.Helpers
{
    public static class DiaLaboralHelper
    {
        // Devuelve la fecha de inicio del día laboral
        public static DateTime ObtenerFechaLaboral(DateTime ahora)
        {
            var inicioDia = new DateTime(ahora.Year, ahora.Month, ahora.Day, 7, 0, 0);

            if (ahora < inicioDia)
            {
                // Si estamos antes de las 7:00, pertenece al día laboral anterior
                inicioDia = inicioDia.AddDays(-1);
            }

            return inicioDia;
        }

        // Devuelve el turno actual según la hora
        public static string ObtenerTurno(DateTime ahora)
        {
            var hora = ahora.TimeOfDay;

            if (hora >= new TimeSpan(7, 0, 0) && hora <= new TimeSpan(15, 30, 0))
                return "Turno 1";
            if (hora >= new TimeSpan(15, 31, 0) && hora <= new TimeSpan(23, 44, 0))
                return "Turno 2";

            return "Turno 3"; // 23:45 a 6:59
        }
    }
}