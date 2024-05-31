using BotPeliculas.Services;
using BotPeliculas.Models;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using System.Net;


namespace BotPeliculas.Tests
{
    [TestFixture]
    public class PeliculasServiceTests
    {
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private PeliculasService _service;

        [SetUp]
        public void Setup()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _service = new PeliculasService(_httpClient);
        }

        private void SetupHttpResponse(string jsonResponse)
        {
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });
        }

        [Test]
        public async Task MostPopularMovieAsync_ReturnsMovie()
        {
            var genre = "action";
            var jsonResponse = CreateJsonResponse(1);
            SetupHttpResponse(jsonResponse);

            var result = await _service.MostPopularMovieAsync(genre);

            AssertMovie(result, 1);
        }

        [Test]
        public async Task RandomMovieAsync_ReturnsListOfMovies()
        {
            var genre = "comedy";
            var jsonResponse = CreateJsonResponse(10);
            SetupHttpResponse(jsonResponse);

            var result = await _service.RandomMovieAsync(genre);

            AssertMovies(result, 10);
        }

        [Test]
        public async Task TopTenMoviesOfGenderAsync_ReturnsTopTenMovies()
        {
            var genre = "drama";
            var jsonResponse = CreateJsonResponse(10);
            SetupHttpResponse(jsonResponse);

            var result = await _service.TopTenMoviesOfGenderAsync(genre);

            AssertMovies(result, 10);
        }

        [Test]
        public async Task TopTenMoviesOfTheDayAsync_ReturnsTopTenTrendingMovies()
        {
            var jsonResponse = CreateJsonResponse(10);
            SetupHttpResponse(jsonResponse);

            var result = await _service.TopTenMoviesOfTheDayAsync();

            AssertMovies(result, 10);
        }

        [Test]
        public void MostPopularMovieAsync_ThrowsException_WhenNoMoviesFound()
        {
            var genre = "action";
            var jsonResponse = CreateEmptyJsonResponse();
            SetupHttpResponse(jsonResponse);

            Assert.ThrowsAsync<Exception>(async () => await _service.MostPopularMovieAsync(genre), "No hay Pelicula popular");
        }

        private string CreateJsonResponse(int numberOfMovies)
        {
            var moviesArray = new JArray();
            for (int i = 1; i <= numberOfMovies; i++)
            {
                moviesArray.Add(CreateMovieJson(i));
            }

            var jsonResponse = new JObject
            {
                ["results"] = moviesArray
            }.ToString();

            return jsonResponse;
        }

        private JObject CreateMovieJson(int id)
        {
            return new JObject
            {
                ["backdrop_path"] = $"/path{id}.jpg",
                ["id"] = id,
                ["original_title"] = $"Original Title {id}",
                ["overview"] = $"Overview {id}",
                ["poster_path"] = $"/poster{id}.jpg",
                ["media_type"] = "movie",
                ["adult"] = false,
                ["title"] = $"Title {id}",
                ["original_language"] = "en",
                ["genre_ids"] = new JArray { 1, 2, 3 },
                ["popularity"] = 8.5 + id,
                ["release_date"] = "2023-05-31",
                ["video"] = false,
                ["vote_average"] = 7.5 + id,
                ["vote_count"] = 1000 + id
            };
        }

        private string CreateEmptyJsonResponse()
        {
            var jsonResponse = new JObject
            {
                ["results"] = new JArray()
            }.ToString();

            return jsonResponse;
        }

        private void AssertMovie(Pelicula movie, int expectedId)
        {
            Assert.NotNull(movie);
            Assert.AreEqual(expectedId, movie.Id);
            Assert.AreEqual($"Original Title {expectedId}", movie.OriginalTitle);
        }

        private void AssertMovies(List<Pelicula> movies, int expectedCount)
        {
            Assert.NotNull(movies);
            Assert.AreEqual(expectedCount, movies.Count);
            for (int i = 1; i <= expectedCount; i++)
            {
                AssertMovie(movies[i - 1], i);
            }
        }
    }
}
