using BotPeliculas.Models;

namespace BotPeliculas.Interfaces
{
    public interface IPeliculasService
    {
        Task<List<Pelicula>> TopTenMoviesOfTheDayAsync();
        Task<List<Pelicula>> TopTenMoviesOfGenderAsync(string genre);
        Task<List<Pelicula>> RandomMovieAsync(string genre);
        Task<Pelicula> MostPopularMovieAsync(string genre);
        Task<string> GetTrailerUrlAsync(int movieId);

    }
}
