using Batch.Data;
using Batch.Models;
using Batch.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // ✅ GET: Vista principal
        public IActionResult Index()
        {
            var vm = new UsuariosViewModel
            {
                Usuarios = _context.Usuarios
                    .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                    .Select(u => new UsuarioItemViewModel
                    {
                        Id = u.Id,
                        Nombre = u.Nombre,
                        UsuarioLogin = u.UsuarioLogin,
                        RolNombre = u.UsuarioRoles.FirstOrDefault().Rol.Nombre
                    })
                    .ToList()
            };

            return View(vm);
        }

        // ✅ GET: Modal Crear
        public IActionResult ModalCrear()
        {
            var vm = new UsuarioFormViewModel
            {
                RolesDisponibles = _context.Roles.ToList()
            };

            return PartialView("_UsuarioModal", vm);
        }

        // ✅ GET: Modal Editar
        public IActionResult ModalEditar(int id)
        {
            var usuario = _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .FirstOrDefault(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            var vm = new UsuarioFormViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                UsuarioLogin = usuario.UsuarioLogin,
                RolId = usuario.UsuarioRoles.FirstOrDefault()?.RolId ?? 0,
                RolesDisponibles = _context.Roles.ToList()
            };

            return PartialView("_UsuarioModal", vm);
        }

        // ✅ POST: Guardar (crear o editar)
        [HttpPost]
        public IActionResult Guardar([FromBody] UsuarioFormViewModel model)
        {
            if (model == null)
                return BadRequest(new { success = false, message = "Datos inválidos" });

            if (string.IsNullOrWhiteSpace(model.Nombre) ||
                string.IsNullOrWhiteSpace(model.UsuarioLogin) ||
                model.RolId <= 0)
            {
                return BadRequest(new { success = false, message = "Todos los campos son obligatorios." });
            }

            // ✅ CREAR
            if (model.Id == 0)
            {
                if (_context.Usuarios.Any(u => u.UsuarioLogin == model.UsuarioLogin))
                    return BadRequest(new { success = false, message = "El usuario ya existe." });

                if (string.IsNullOrWhiteSpace(model.Password))
                    return BadRequest(new { success = false, message = "La contraseña es obligatoria." });

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

            // ✅ EDITAR
            var existente = _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .FirstOrDefault(u => u.Id == model.Id);

            if (existente == null)
                return NotFound(new { success = false, message = "Usuario no encontrado." });

            if (_context.Usuarios.Any(u => u.UsuarioLogin == model.UsuarioLogin && u.Id != model.Id))
                return BadRequest(new { success = false, message = "El usuario ya existe." });

            existente.Nombre = model.Nombre;
            existente.UsuarioLogin = model.UsuarioLogin;

            // ✅ Cambiar contraseña solo si se envió
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                existente.PasswordHash = _passwordHasher.HashPassword(existente, model.Password);
            }

            // ✅ Actualizar rol
            var rolActual = existente.UsuarioRoles.FirstOrDefault();

            if (rolActual == null)
            {
                _context.UsuarioRoles.Add(new UsuarioRol
                {
                    UsuarioId = existente.Id,
                    RolId = model.RolId
                });
            }
            else if (rolActual.RolId != model.RolId)
            {
                rolActual.RolId = model.RolId;
            }

            _context.SaveChanges();

            return Ok(new { success = true, message = "Usuario actualizado correctamente" });
        }
    }
}