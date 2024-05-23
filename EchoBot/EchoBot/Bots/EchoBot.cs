using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly HttpClient _httpClient;
        private string apiKey = "a45cc75d9a7cde74a34d466138e3dd6d";

        private static readonly Dictionary<string, string> MoodToGenre = new Dictionary<string, string>
        {
            { "triste", "18" },
            { "feliz", "35" },
            { "enojado", "28" },
            { "romantico", "10749" },
            { "nervioso", "27" },
            { "curioso", "99" }
        };

        public EchoBot(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var userMessage = turnContext.Activity.Text.ToLower();

            if (int.TryParse(userMessage, out int selectedOption))
            {
                switch (selectedOption)
                {
                    case 1:
                        await RecommendMoviesOfTheDay(turnContext, cancellationToken);
                        break;
                    case 2:
                        await ShowMoodSelectionCard(turnContext, cancellationToken);
                        break;
                    default:
                        await turnContext.SendActivityAsync(MessageFactory.Text("Opción no válida. Por favor, selecciona una opción válida."), cancellationToken);
                        await SendWelcomeMessage(turnContext, cancellationToken);
                        break;
                }
            }
            else if (userMessage.Contains("_most_viewed") || userMessage.Contains("_top_10") || userMessage.Contains("_random"))
            {
                var mood = userMessage.Split('_')[0];
                if (MoodToGenre.ContainsKey(mood))
                {
                    await RecommendMoviesByMood(turnContext, cancellationToken, userMessage);
                    await ShowYesNoOptionsCard(turnContext, cancellationToken, "¿Quieres continuar?");
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Opción no válida. Por favor, selecciona una opción válida."), cancellationToken);
                }
            }
            else if (MoodToGenre.Keys.Any(m => userMessage.Contains(m)))
            {
                var mood = MoodToGenre.Keys.FirstOrDefault(m => userMessage.Contains(m));
                await ShowMoodOptionsCard(turnContext, cancellationToken, mood);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Por favor, selecciona una opción válida."), cancellationToken);
            }
        }


        private async Task ShowMoodOptionsCard(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken, string mood)
        {
            var reply = MessageFactory.Attachment(new List<Attachment>());
            var card = new HeroCard
            {
                Title = $"Opciones para {mood}",
                Buttons = new List<CardAction>
        {
            new CardAction { Title = "Mostrar la película más vista", Type = ActionTypes.ImBack, Value = $"{mood}_most_viewed" },
            new CardAction { Title = "Mostrar el top 10", Type = ActionTypes.ImBack, Value = $"{mood}_top_10" },
            new CardAction { Title = "Mostrar una película random", Type = ActionTypes.ImBack, Value = $"{mood}_random" }
        }
            };

            reply.Attachments.Add(card.ToAttachment());
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        private async Task RecommendMoviesByMood(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken, string mood)
        {
            var moodSelected = mood.Split('_')[0];
            if (MoodToGenre.TryGetValue(moodSelected, out string genreId))
            {
                var userMessage = turnContext.Activity.Text.ToLower();

                if (userMessage.Contains("most_viewed"))
                {
                    await ShowMostViewedMovie(turnContext, cancellationToken, genreId, moodSelected);
                }
                else if (userMessage.Contains("top_10"))
                {
                    await ShowTop10Movies(turnContext, cancellationToken, genreId, moodSelected);
                }
                else if (userMessage.Contains("random"))
                {
                    await ShowRandomMovie(turnContext, cancellationToken, genreId, moodSelected);
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Opción no valida. Por favor selecciona una opcion valida."), cancellationToken);
                }
            }
        }

        private async Task ShowMostViewedMovie(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken, string genreId, string mood)
        {
            string url = $"https://api.themoviedb.org/3/discover/movie?api_key={apiKey}&with_genres={genreId}&sort_by=popularity.desc";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            var mostViewedMovie = json["results"].FirstOrDefault();
            if (mostViewedMovie != null)
            {
                var replyText = $"La película más vista para cuando estas {mood} es:\n";
                replyText += $"- {mostViewedMovie["title"]}\n";
                var posterPath = (string)mostViewedMovie["poster_path"];
                if (!string.IsNullOrEmpty(posterPath))
                {
                    replyText += $"![{mostViewedMovie["title"]}](https://image.tmdb.org/t/p/w300_and_h450_bestv2/{posterPath})";
                }
                else
                {
                    replyText += "No hay poster disponible para esta película.";
                }

                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"No encontré películas para el estado de ánimo: {mood}", $"No encontré películas para el estado de ánimo: {mood}"), cancellationToken);
            }
        }


        private async Task ShowTop10Movies(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken, string genreId, string mood)
        {
            string url = $"https://api.themoviedb.org/3/discover/movie?api_key={apiKey}&with_genres={genreId}&sort_by=popularity.desc";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            var movies = json["results"].Take(10).ToArray();
            if (movies.Length > 0)
            {
                var replyText = $"Top 10 películas para cuando estas {mood}:\n";
                foreach (var movie in movies)
                {
                    replyText += $"- {movie["title"]}\n";
                    var posterPath = (string)movie["poster_path"];
                    if (!string.IsNullOrEmpty(posterPath))
                    {
                        replyText += $"![{movie["title"]}](https://image.tmdb.org/t/p/w300_and_h450_bestv2/{posterPath})\n";
                    }
                    else
                    {
                        replyText += "No hay poster disponible para esta película.\n";
                    }
                }

                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"No encontré películas para el estado de ánimo: {mood}", $"No encontré películas para el estado de ánimo: {mood}"), cancellationToken);
            }
        }

        private async Task ShowRandomMovie(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken, string genreId, string mood)
        {
            string url = $"https://api.themoviedb.org/3/discover/movie?api_key={apiKey}&with_genres={genreId}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            var movies = json["results"].ToArray();
            if (movies.Length > 0)
            {
                var random = new Random();
                var randomMovie = movies[random.Next(movies.Length)];

                var replyText = $"Recomiendo esta película para cuando estas {mood}:\n";
                replyText += $"- {randomMovie["title"]}\n";
                var posterPath = (string)randomMovie["poster_path"];
                if (!string.IsNullOrEmpty(posterPath))
                {
                    replyText += $"![{randomMovie["title"]}](https://image.tmdb.org/t/p/w300_and_h450_bestv2/{posterPath})";
                }
                else
                {
                    replyText += "No hay poster disponible para esta película.";
                }

                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"No encontré películas para el estado de ánimo: {mood}", $"No encontré películas para el estado de ánimo: {mood}"), cancellationToken);
            }
        }



        private async Task ShowMoodSelectionCard(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Attachment(new List<Attachment>());
            var card = new HeroCard
            {
                Title = "Selecciona tu estado de animo",
                Buttons = MoodToGenre.Keys.Select(mood => new CardAction
                {
                    Title = mood,
                    Type = ActionTypes.ImBack,
                    Value = mood
                }).ToList()
            };

            reply.Attachments.Add(card.ToAttachment());
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        private async Task RecommendMoviesOfTheDay(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            string url = $"https://api.themoviedb.org/3/trending/movie/day?api_key={apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            var movies = json["results"].ToArray();
            if (movies.Length > 0)
            {
                var replyText = "Peliculas del dia:\n";
                for (int i = 0; i < Math.Min(5, movies.Length); i++)
                {
                    var movie = movies[i];
                    replyText += $"- {movie["title"]}\n";
                    var posterPath = (string)movie["poster_path"];
                    if (!string.IsNullOrEmpty(posterPath))
                    {
                        replyText += $"![{movie["title"]}](https://image.tmdb.org/t/p/w300_and_h450_bestv2/{posterPath})\n";
                    }
                    else
                    {
                        replyText += "No hay poster disponible para esta pelicula.\n";
                    }
                }

                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("No encontre peliculas del dia.", "No encontre peliculas del dia."), cancellationToken);
            }

        }


        private async Task SendWelcomeMessage(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "¡Hola y bienvenido! Por favor, selecciona una opción:\n" +
                              "1. Ver películas del día\n" +
                              "2. Recomendar películas según mi estado de ánimo";
            await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
        }

        private async Task SendContinueMessage(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "1. Ver películas del día\n" +
                              "2. Recomendar películas según mi estado de ánimo";
            await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await SendWelcomeMessage(turnContext, cancellationToken);
                }
            }
        }

        private async Task ShowYesNoOptionsCard(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken, string prompt)
        {
            var reply = MessageFactory.Attachment(new List<Attachment>());
            var card = new HeroCard
            {
                Title = prompt,
                Buttons = new List<CardAction>
        {
            new CardAction { Title = "Sí", Type = ActionTypes.ImBack, Value = "yes" },
            new CardAction { Title = "No", Type = ActionTypes.ImBack, Value = "no" }
        }
            };

            reply.Attachments.Add(card.ToAttachment());
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}
