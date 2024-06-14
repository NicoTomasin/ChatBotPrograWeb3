using BotPeliculas.Interfaces;
using BotPeliculas.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotPeliculas.Bot.Dialogs;

public class ProximasPeliculas : ComponentDialog
{
    private readonly IPeliculasService _peliculasService;
    public ProximasPeliculas(IPeliculasService peliculasService) : base(nameof(ProximasPeliculas))
    {
        var waterfallSteps = new WaterfallStep[]
        {
            MostrarProximasPeliculasAsync,
            EndDialogAsync
        };

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        _peliculasService = peliculasService;
    }

    private async Task<DialogTurnResult> MostrarProximasPeliculasAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var movies = await _peliculasService.GetUpcomingMoviesAsync();
        var heroCards = movies.Select(movie =>
        {
            var trailerUrl = _peliculasService.GetTrailerUrlAsync(movie.Id).Result;
            return CreateHeroCard(movie, trailerUrl);
        }).ToList();

        var reply = MessageFactory.Carousel(heroCards.Select(card => card.ToAttachment()).ToList());
        await stepContext.Context.SendActivityAsync(reply, cancellationToken);

        return await stepContext.NextAsync(null, cancellationToken);
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
                new CardAction(ActionTypes.OpenUrl, "Ver Tráiler", value: trailerUrl),
            }
        };
    }

    private async Task<DialogTurnResult> EndDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        return await stepContext.ReplaceDialogAsync(nameof(RootDialog), null, cancellationToken);
    }
}

