using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Linq;
using System.Collections.Generic;
using BotPeliculas.Models;
using BotPeliculas;
using Microsoft.BotBuilderSamples;
using BotPeliculas.Interfaces;

public class Top10 : ComponentDialog
{
    private readonly IPeliculasService _peliculasService;
    private string _genero ;
    public Top10(IPeliculasService peliculasService) : base(nameof(Top10))
    {
        var waterfallSteps = new WaterfallStep[]
        {
            ShowPeliculasDelDiaAsync,
            EndDialogAsync
        };

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        _peliculasService = peliculasService;
    }
    private async Task<DialogTurnResult> ShowPeliculasDelDiaAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var userSelection = stepContext.Options as (string Mood, string Genre)?;
        var mood = userSelection?.Mood;
        var genre = userSelection?.Genre;

        string message = !string.IsNullOrEmpty(genre)
            ? $"Este es el Top de películas para estado de ánimo: {mood}"
            : "Estas son las 10 películas más vistas:";

        await stepContext.Context.SendActivityAsync(message, cancellationToken: cancellationToken);

        var heroCards = await GetHeroCardsAsync(genre);
        var reply = MessageFactory.Carousel(heroCards.Select(card => card.ToAttachment()).ToList());
        await stepContext.Context.SendActivityAsync(reply, cancellationToken);

        return await stepContext.NextAsync(null, cancellationToken);
    }

    private async Task<List<HeroCard>> GetHeroCardsAsync(string genre)
    {
        var movies = string.IsNullOrEmpty(genre)
            ? await _peliculasService.TopTenMoviesOfTheDayAsync()
            : await _peliculasService.TopTenMoviesOfGenderAsync(genre);

        var heroCards = movies.Select(movie =>
        {
            var trailerUrl = _peliculasService.GetTrailerUrlAsync(movie.Id).Result;
            return CreateHeroCard(movie, trailerUrl);
        }).ToList();

        return heroCards;
    }

    private HeroCard CreateHeroCard(Pelicula movie, string trailerUrl)
    {
        var imageUrl = Urls.GetImageUrl(movie.PosterPath);

        return new HeroCard
        {
            Title = movie.Title,
            Images = new List<CardImage> { new CardImage(imageUrl) },
            Buttons = new List<CardAction>
            {
                new CardAction(ActionTypes.PlayVideo, "Ver Tráiler", value: trailerUrl),

            }
        };

    }

    private async Task<DialogTurnResult> EndDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        return await stepContext.ReplaceDialogAsync(nameof(RootDialog), null, cancellationToken);
    }


}

