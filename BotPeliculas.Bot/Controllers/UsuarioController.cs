using BotPelicualas.Logica;
using Microsoft.AspNetCore.Mvc;

namespace BotPeliculas.Bot.Controllers;

public class UsuarioController : Controller
{
    private static UsuarioServicio _usuarioServicio;
    public IActionResult Index()
    {
        return View();
    }
}
