
namespace BotPeliculas
{
    public static class Urls
    {
        private const string BaseDiscoverUrl = "https://api.themoviedb.org/3/discover/movie?api_key=";
        private const string BaseImageUrl = "https://image.tmdb.org/t/p/w300_and_h450_bestv2/";
        private const string ApiBaseUrl = "https://api.themoviedb.org/3";
        private const string ApiKey = "a45cc75d9a7cde74a34d466138e3dd6d";
        public static string GetApiKey()
        {
            return ApiKey;
        }

        public static string GetImageUrl(string posterPath)
        {
            return $"{BaseImageUrl}{posterPath}";
        }

        public static string GetMoviesByGenreUrl(string apiKey, string genre)
        {
            return $"{ApiBaseUrl}/discover/movie?api_key={apiKey}&with_genres={genre}&sort_by=popularity.desc";
        }

        public static string GetTrendingMoviesUrl(string apiKey)
        {
            return $"{ApiBaseUrl}/trending/movie/day?api_key={apiKey}";
        }
        public static string GetDiscoverUrl(string apiKey, string genre)
        {
            return $"{BaseDiscoverUrl}{apiKey}&with_genres={genre}&sort_by=vote_average.desc&vote_count.gte=100";
        }
        public static string GetMostPopularUrl(string apiKey, string genre)
        {
            return $"{BaseDiscoverUrl}{apiKey}&with_genres={genre}&sort_by=vote_average.desc&vote_count.gte=10000";
        }

        public static string GetTrailerUrl(int movieId, string ApiKey)
        {
            return $"{ApiBaseUrl}/movie/{movieId}/videos?api_key={ApiKey}";
        }

        public static string GetUpcomingMoviesUrl(string apiKey)
        {
            return $"{ApiBaseUrl}/movie/upcoming?api_key={apiKey}";
        }

    }
}
