using Batch.Data;
using Batch.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Necesario para que Session funcione
builder.Services.AddDistributedMemoryCache();

// ✅ Configuración correcta de Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// ✅ Identity
builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";          // ✅ Login personalizado
    options.AccessDeniedPath = "/Auth/AccessDenied"; // ✅ Vista de acceso denegado
});


builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
});

// ✅ HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// ✅ DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BatchSystemConnection")));

var app = builder.Build();

// ✅ ORDEN CORRECTO DE MIDDLEWARES

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Session SIEMPRE antes de Authentication/Authorization
app.UseSession();

// ✅ Tus middlewares personalizados (si los necesitas)
//app.UseMiddleware<TimeZoneMiddleware>();

// ✅ Identity
app.UseAuthentication();
app.UseAuthorization();

// ✅ Crear roles y usuario admin dentro de un scope
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Crear roles
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "Operador", "Supervisor" };

    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
            await roleManager.CreateAsync(new IdentityRole(rol));
    }

    // Crear usuario admin
    var userManager = services.GetRequiredService<UserManager<Usuario>>();
    var admin = await userManager.FindByNameAsync("admin");

    if (admin == null)
    {
        admin = new Usuario
        {
            UserName = "admin",
            Nombre = "Administrador"
        };

        await userManager.CreateAsync(admin, "1234");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

// ✅ Rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Batch}/{action=Crear}/{id?}");

app.Run();