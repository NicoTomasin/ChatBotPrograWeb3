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
        
        _usuarioServicio.AgregarUsuario(usuario);

        return RedirectToAction("Login");
    }
    public IActionResult Login() 
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string NombreUsuario, string Contrasenia)
    {
        var usuarioBuscado = _usuarioServicio.BuscarPorNombreDeUsuario(NombreUsuario);
        if (usuarioBuscado.Password == Contrasenia)
        {
            return RedirectToAction("Index", "Home");
        }
            return RedirectToAction("Login");
        
    }
    
}
