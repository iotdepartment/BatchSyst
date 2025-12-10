using Batch.Data;
using Batch.Models;
using Batch.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public AuthController(AppDbContext context, IPasswordHasher<Usuario> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }


    [HttpPost]
    [AllowAnonymous]

    public IActionResult Login([FromBody] LoginViewModel model)
    {
        if (model == null)
            return BadRequest(new { success = false, message = "Datos inválidos" });

        var usuario = _context.Usuarios
            .FirstOrDefault(u => u.UsuarioLogin == model.UsuarioLogin);

        if (usuario == null)
            return BadRequest(new { success = false, message = "Usuario o contraseña incorrectos" });

        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, model.Password);

        if (resultado == PasswordVerificationResult.Failed)
            return BadRequest(new { success = false, message = "Usuario o contraseña incorrectos" });

        // ✅ Guardar sesión
        HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
        HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);

        return Ok(new { success = true, message = "Login exitoso" });
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return Ok(new { success = true, message = "Sesión cerrada correctamente" });
    }
}