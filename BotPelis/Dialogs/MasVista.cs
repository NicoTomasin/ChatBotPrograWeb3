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

public class MasVista : ComponentDialog
{
    private readonly HttpClient _httpClient = new HttpClient();
    private string apiKey = "a45cc75d9a7cde74a34d466138e3dd6d";
    private string _genero ;
    public MasVista() : base(nameof(MasVista))
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
            await stepContext.Context.SendActivityAsync($"Esta es la pelicula mas vista de tu mood: {mood}", cancellationToken: cancellationToken);
        }
        else
        {
            throw new Exception("genero inexistente");
        }

        var heroCard = await MostPopularMovieAsync(genre);
        var reply = MessageFactory.Attachment(heroCard.ToAttachment());
        await stepContext.Context.SendActivityAsync(reply, cancellationToken);


        return await stepContext.NextAsync(null, cancellationToken);
    }
    private async Task<HeroCard> MostPopularMovieAsync(string genre)
    {
        string url = $"https://api.themoviedb.org/3/discover/movie?api_key={apiKey}&with_genres={genre}&sort_by=vote_average.desc&vote_count.gte=10000";

        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        JObject json = JObject.Parse(responseBody);

        var movie = json["results"].FirstOrDefault();

        if (movie != null)
        {
            var title = movie["title"].ToString();
            var posterPath = movie["poster_path"].ToString();
            var imageUrl = $"https://image.tmdb.org/t/p/w300_and_h450_bestv2/{posterPath}";

            return new HeroCard
            {
                Title = title,
                Images = new List<CardImage> { new CardImage(imageUrl) }
            };
        }
        else
        {
            return new HeroCard { Title = "No se encontraron películas" };
        }
    }


    private async Task<DialogTurnResult> EndDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        await stepContext.Context.SendActivityAsync("Espero que hayas disfrutado de las recomendaciones. ¿Hay algo más en lo que te pueda ayudar?", cancellationToken: cancellationToken);
        return await stepContext.EndDialogAsync(null, cancellationToken);
    }


}

