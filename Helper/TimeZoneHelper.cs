namespace Batch.Helper
{
    public static class TimeZoneHelper
    {
        private static readonly TimeZoneInfo ZonaMatamoros =
            TimeZoneInfo.FindSystemTimeZoneById("America/Matamoros");

        public static DateTime Ahora()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ZonaMatamoros);
        }
    }

}
