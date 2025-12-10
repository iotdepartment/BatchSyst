using Batch.Data;
using Batch.Migrations;
using Batch.Models;
using Batch.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Batch.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuarioController(AppDbContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // ✅ GET: Mostrar formulario
        public IActionResult Crear()
        {
            var vm = new CrearUsuarioViewModel
            {
                RolesDisponibles = _context.Roles.ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] CrearUsuarioViewModel model)
        {
            if (model == null)
                return BadRequest(new { success = false, message = "Datos inválidos" });

            // ✅ Validación: usuario duplicado
            bool existeUsuario = _context.Usuarios
                .Any(u => u.UsuarioLogin == model.UsuarioLogin);

            if (existeUsuario)
            {
                return BadRequest(new { success = false, message = "El usuario ya existe. Elija otro nombre de usuario." });
            }

            // ✅ Validación de campos obligatorios
            if (string.IsNullOrWhiteSpace(model.Nombre) ||
                string.IsNullOrWhiteSpace(model.UsuarioLogin) ||
                string.IsNullOrWhiteSpace(model.Password) ||
                model.RolId <= 0)
            {
                return BadRequest(new { success = false, message = "Todos los campos son obligatorios." });
            }

            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                UsuarioLogin = model.UsuarioLogin,
                Activo = true
            };

            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, model.Password);

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            _context.UsuarioRoles.Add(new UsuarioRol
            {
                UsuarioId = usuario.Id,
                RolId = model.RolId
            });

            _context.SaveChanges();

            return Ok(new { success = true, message = "Usuario creado correctamente" });
        }
    }
}
