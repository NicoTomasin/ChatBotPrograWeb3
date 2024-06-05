using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using BotPeliculas;
using Microsoft.BotBuilderSamples;
using BotPeliculas.Interfaces;

public class MasVista : ComponentDialog
{
    private readonly IPeliculasService _peliculasService;
    private string _genero ;
    public MasVista(IPeliculasService peliculasService) : base(nameof(MasVista))
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
        var movie = await _peliculasService.MostPopularMovieAsync(genre);
        if (movie == null)
        {
            return new HeroCard { Title = "No se encontraron películas" };
        }
        var imageUrl = Urls.GetImageUrl(movie.PosterPath);
        var trailerUrl = await _peliculasService.GetTrailerUrlAsync(movie.Id);
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

