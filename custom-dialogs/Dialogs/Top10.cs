using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Linq;
using System;
using System.Collections.Generic;

public class Top10 : ComponentDialog
{
    private readonly HttpClient _httpClient = new HttpClient();
    private string apiKey = "a45cc75d9a7cde74a34d466138e3dd6d";
    private string _genero ;
    public Top10() : base(nameof(Top10))
    {
        var waterfallSteps = new WaterfallStep[]
        {
            ShowPeliculasDelDiaAsync,
            EndDialogAsync
        };

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
    }
    private async Task<DialogTurnResult> ShowPeliculasDelDiaAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var userSelection = stepContext.Options as (string Mood, string Genre)?;
        var mood = userSelection?.Mood;
        var genre = userSelection?.Genre;
        if (!string.IsNullOrEmpty(genre))
        {
            await stepContext.Context.SendActivityAsync($"Este es el Top de películas para estado de ánimo: {mood}", cancellationToken: cancellationToken);
        }
        else
        {
            await stepContext.Context.SendActivityAsync("Estas son las 10 películas más vistas:", cancellationToken: cancellationToken);
        }

        var heroCards = await TopTenMoviesOfTheDayAsync(genre);
        var reply = MessageFactory.Carousel(heroCards.Select(card => card.ToAttachment()).ToList());
        await stepContext.Context.SendActivityAsync(reply, cancellationToken);

        return await stepContext.NextAsync(null, cancellationToken);
    }
    private async Task<List<HeroCard>> TopTenMoviesOfTheDayAsync(string genre = null)
    {
        string url = string.IsNullOrEmpty(genre)
     ? $"https://api.themoviedb.org/3/trending/movie/day?api_key={apiKey}"
     : $"https://api.themoviedb.org/3/discover/movie?api_key={apiKey}&with_genres={genre}&sort_by=popularity.desc";

        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        JObject json = JObject.Parse(responseBody);

        var movies = json["results"].Take(10).ToArray();
        var heroCards = new List<HeroCard>();

        foreach (var movie in movies)
        {
            var title = movie["title"].ToString();
            var posterPath = movie["poster_path"].ToString();
            var imageUrl = $"https://image.tmdb.org/t/p/w300_and_h450_bestv2/{posterPath}";

            var heroCard = new HeroCard
            {
                Title = title,
                Images = new List<CardImage> { new CardImage(imageUrl) }
            };

            heroCards.Add(heroCard);
        }

        return heroCards;
    }

    private async Task<DialogTurnResult> EndDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        return await stepContext.EndDialogAsync(null, cancellationToken);
    }


}

