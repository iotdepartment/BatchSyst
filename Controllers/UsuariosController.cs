using Batch.Models;
using Batch.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class UsuariosController : Controller
{
    private readonly UserManager<Usuario> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsuariosController(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // ✅ LISTADO
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var usuarios = _userManager.Users.ToList();
        var lista = new List<UsuarioConRolViewModel>();

        foreach (var u in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(u);

            lista.Add(new UsuarioConRolViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Nombre = u.Nombre,
                Rol = roles.FirstOrDefault() ?? "Sin rol"
            });
        }

        return View(lista);
    }

    // ✅ CREAR USUARIO
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearUsuarioViewModel model)
    {
        var usuario = new Usuario
        {
            UserName = model.UserName,
            Nombre = model.Nombre
        };

        var result = await _userManager.CreateAsync(usuario, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(usuario, model.Rol);

        return Ok(new { success = true });
    }

    // ✅ OBTENER USUARIO PARA EDITAR
    [HttpGet]
    public async Task<IActionResult> Obtener(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(usuario);

        return Ok(new
        {
            id = usuario.Id,
            userName = usuario.UserName,
            nombre = usuario.Nombre,
            rol = roles.FirstOrDefault()
        });
    }

    // ✅ EDITAR USUARIO
    [HttpPost]
    public async Task<IActionResult> Editar([FromBody] EditarUsuarioViewModel model)
    {
        var usuario = await _userManager.FindByIdAsync(model.Id);

        usuario.UserName = model.UserName;
        usuario.Nombre = model.Nombre;

        await _userManager.UpdateAsync(usuario);

        var rolesActuales = await _userManager.GetRolesAsync(usuario);
        await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
        await _userManager.AddToRoleAsync(usuario, model.Rol);

        return Ok(new { success = true });
    }

    // ✅ ELIMINAR USUARIO
    [HttpPost]
    public async Task<IActionResult> Eliminar([FromBody] string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null)
            return NotFound();

        await _userManager.DeleteAsync(usuario);

        return Ok(new { success = true });
    }
}