using System;

public static class TimeZoneHelper
{
    private static readonly TimeZoneInfo ZonaMatamoros;

    static TimeZoneHelper()
    {
        try
        {
            // ✅ Linux
            ZonaMatamoros = TimeZoneInfo.FindSystemTimeZoneById("America/Matamoros");
        }
        catch
        {
            // ✅ Windows
            ZonaMatamoros = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
        }
    }

    public static DateTime Ahora()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ZonaMatamoros);
    }
}