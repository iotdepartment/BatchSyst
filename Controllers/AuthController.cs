using Batch.Models;
using Batch.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    private readonly SignInManager<Usuario> _signInManager;
    private readonly UserManager<Usuario> _userManager;

    public AuthController(SignInManager<Usuario> signInManager, UserManager<Usuario> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // ✅ GET Login
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Usuarios");

        return View();
    }

    // ✅ POST Login
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(
            model.UsuarioLogin,
            model.Password,
            false,
            false
        );

        if (!result.Succeeded)
            return BadRequest(new { success = false, message = "Usuario o contraseña incorrectos" });

        return Ok(new { success = true });
    }

    // ✅ LOGOUT
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}