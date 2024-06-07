using BotPeliculas.Models.EF;
using BotPeliculas.Services;
using Microsoft.AspNetCore.Mvc;

namespace BotPeliculas.Bot.Controllers;

public class UsuarioController : Controller
{
    private IUsuarioServicio _usuarioServicio;

    public UsuarioController(IUsuarioServicio usuarioServicio)
    {
        _usuarioServicio = usuarioServicio;
    }
    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registro(Usuario usuario)
    {

        if (!ModelState.IsValid)
            return View(usuario);

        var usuarioExistente = _usuarioServicio.BuscarPorNombreDeUsuario(usuario.UserName);
        if (usuarioExistente != null)
        {
            ModelState.AddModelError("UserName", "El nombre de usuario ya existe");
            return View(usuario);
        }

        _usuarioServicio.AgregarUsuario(usuario);

        return RedirectToAction("Login");
    }
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string UserName, string Contrasenia)
    {
        var usuarioBuscado = _usuarioServicio.BuscarPorNombreDeUsuario(UserName);
        if (usuarioBuscado != null && usuarioBuscado.Password == Contrasenia)
        {
            HttpContext.Session.SetString("UsuarioLogueado", usuarioBuscado.UserName);
            return RedirectToAction("Index", "Home");
        }
        return RedirectToAction("Login");

    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("UsuarioLogueado");
        return RedirectToAction("Login");
    }

}
