using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        // ✅ Rutas que NO requieren login
        var rutasPublicas = new[]
        {
            "/auth/login",
            "/auth/logout",
            "/css", "/js", "/images", "/lib"
        };

        bool esPublica = rutasPublicas.Any(r => path.StartsWith(r));

        if (!esPublica)
        {
            // ✅ Validar sesión
            if (context.Session.GetInt32("UsuarioId") == null)
            {
                context.Response.Redirect("/Auth/Login");
                return;
            }
        }

        await _next(context);
    }
}