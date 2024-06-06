using BotPeliculas.FrontEnd.Models;
using BotPeliculas.Interfaces;
using BotPeliculas.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BotPeliculas.FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPeliculasService _peliculasService;

        public HomeController(ILogger<HomeController> logger, IPeliculasService peliculasService)
        {
            _logger = logger;
            _peliculasService = peliculasService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var movies = await _peliculasService.TopTenMoviesOfTheDayAsync();
            ViewBag.Movies = movies;
            return View();
        }

        public async Task<IActionResult> GetTrailerUrl(int movieId)
        {
            var trailerUrl = await _peliculasService.GetTrailerUrlAsync(movieId);
            return Ok(trailerUrl);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
