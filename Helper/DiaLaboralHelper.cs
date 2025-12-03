using System;

namespace Batch.Helpers
{
    public static class DiaLaboralHelper
    {
        // Devuelve la fecha de inicio del día laboral (7:00 AM)
        public static DateTime ObtenerFechaLaboral(DateTime fecha)
        {
            var inicioDia = new DateTime(fecha.Year, fecha.Month, fecha.Day, 7, 0, 0);

            if (fecha < inicioDia)
            {
                // Si estamos antes de las 7:00, pertenece al día laboral anterior
                inicioDia = inicioDia.AddDays(-1);
            }

            return inicioDia;
        }

        // Devuelve el turno en el que se creó el batch
        public static string ObtenerTurno(DateTime fecha)
        {
            var hora = fecha.TimeOfDay;

            // Turno 1: 07:00 a 15:30
            if (hora >= new TimeSpan(7, 0, 0) && hora < new TimeSpan(15, 30, 1))
                return "1";

            // Turno 2: 15:30 a 23:45
            if (hora >= new TimeSpan(15, 30, 0) && hora < new TimeSpan(23, 45, 0))
                return "2";

            // Turno 3: 23:45 a 06:59
            return "3";
        }
    }
}