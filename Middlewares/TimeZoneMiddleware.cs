//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;

//public class TimeZoneMiddleware
//{
//    private readonly RequestDelegate _next;

//    public TimeZoneMiddleware(RequestDelegate next)
//    {
//        _next = next;
//    }

//    public async Task Invoke(HttpContext context)
//    {
//        // Puedes guardar la hora actual en Items si quieres usarla en toda la request
//        context.Items["Ahora"] = TimeZoneHelper.Ahora();

//        await _next(context);
//    }
//}