using BotPeliculas.Interfaces;
using BotPeliculas.Models;
using Newtonsoft.Json.Linq;

namespace BotPeliculas.Services
{
    public class PeliculasService : IPeliculasService
    {
        private readonly HttpClient _httpClient;
        private string apiKey = Urls.GetApiKey();
        public PeliculasService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Pelicula> MostPopularMovieAsync(string genre)
        {
            string url = Urls.GetDiscoverUrl(apiKey, genre);
            return await FetchOneMovieAsync(url);
        }

        public async Task<List<Pelicula>> RandomMovieAsync(string genre)
        {
            string url = Urls.GetDiscoverUrl(apiKey, genre);
            return await FetchMoviesAsync(url);
        }

        public async Task<List<Pelicula>> TopTenMoviesOfGenderAsync(string genre)
        {
            string url = Urls.GetMoviesByGenreUrl(apiKey, genre);
            return await FetchMoviesAsync(url);
        }

        public async Task<List<Pelicula>> TopTenMoviesOfTheDayAsync()
        {
            string url = Urls.GetTrendingMoviesUrl(apiKey);
            return await FetchMoviesAsync(url);
        }


        public async Task<string> GetTrailerUrlAsync(int movieId)
        {
            string url = Urls.GetTrailerUrl(movieId, apiKey);
            return await FetchTrailerAsync(url);
        }


        private async Task<Pelicula> FetchOneMovieAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);
            var movieJson = json["results"].FirstOrDefault();
            if (movieJson == null)
            {
                throw new Exception("No hay Pelicula popular");
            }
            return MapJsonToPelicula(movieJson);
        }

        private async Task<List<Pelicula>> FetchMoviesAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            var movies = json["results"].Take(10).Select(MapJsonToPelicula).ToList();
            return movies;
        }

        private async Task<string> FetchTrailerAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);
            return ExtractYouTubeTrailerUrl(json);
        }

        public async Task<List<Pelicula>> GetUpcomingMoviesAsync()
        {
            string url = Urls.GetUpcomingMoviesUrl(apiKey);
            return await FetchMoviesAsync(url);
        }

        private string ExtractYouTubeTrailerUrl(JObject json)
        {
            var trailer = json["results"]
                ?.FirstOrDefault(v =>
                    v["type"]?.Value<string>() == "Trailer" &&
                    v["site"]?.Value<string>() == "YouTube"
                );

            return trailer != null
                ? $"https://www.youtube.com/watch?v={trailer["key"]}"
                : null;
        }
        private Pelicula MapJsonToPelicula(JToken movieJson)
        {
            return new Pelicula
            {
                BackdropPath = (string)movieJson["backdrop_path"],
                Id = (int)movieJson["id"],
                OriginalTitle = (string)movieJson["original_title"],
                Overview = (string)movieJson["overview"],
                PosterPath = (string)movieJson["poster_path"],
                MediaType = (string)movieJson["media_type"],
                Adult = (bool)movieJson["adult"],
                Title = (string)movieJson["title"],
                OriginalLanguage = (string)movieJson["original_language"],
                GenreIds = ((JArray)movieJson["genre_ids"]).Select(id => (int)id).ToList(),
                Popularity = (double)movieJson["popularity"],
                ReleaseDate = DateTime.Parse((string)movieJson["release_date"]),
                Video = (bool)movieJson["video"],
                VoteAverage = (double)movieJson["vote_average"],
                VoteCount = (int)movieJson["vote_count"]
            };
        }
    }
}
