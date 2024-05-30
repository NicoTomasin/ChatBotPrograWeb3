using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Linq;
using System;
using System.Collections.Generic;
using BotPeliculas.Services;
using BotPeliculas;
using BotPeliculas.Models;
using Microsoft.BotBuilderSamples;
using BotPeliculas.Interfaces;

public class PeliculaRandom : ComponentDialog
{
    private readonly IPeliculasService _peliculasService;
    private string _genero;
    public PeliculaRandom(IPeliculasService peliculasService) : base(nameof(PeliculaRandom))
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

        if (userSelection == null || string.IsNullOrEmpty(userSelection.Value.Genre))
        {
            throw new ArgumentException("El género es inexistente o no válido");
        }

        var mood = userSelection.Value.Mood;
        var genre = userSelection.Value.Genre;

        await SendMoodMessageAsync(stepContext, mood, cancellationToken);

        var heroCard = await GetRandomMovieHeroCardAsync(genre);
        await SendHeroCardAsync(stepContext, heroCard, cancellationToken);

        return await stepContext.NextAsync(null, cancellationToken);
    }

    private async Task SendMoodMessageAsync(WaterfallStepContext stepContext, string mood, CancellationToken cancellationToken)
    {
        var message = $"Esta es la película más vista para tu estado de ánimo: {mood}";
        await stepContext.Context.SendActivityAsync(message, cancellationToken: cancellationToken);
    }

    private async Task SendHeroCardAsync(WaterfallStepContext stepContext, HeroCard heroCard, CancellationToken cancellationToken)
    {
        var reply = MessageFactory.Attachment(heroCard.ToAttachment());
        await stepContext.Context.SendActivityAsync(reply, cancellationToken);
    }

    private async Task<HeroCard> GetRandomMovieHeroCardAsync(string genre)
    {
        var movies = await _peliculasService.RandomMovieAsync(genre);

        if (movies.Any())
        {
            var randomMovie = GetRandomMovie(movies);
            return CreateHeroCard(randomMovie);
        }
        else
        {
            return CreateNoMoviesFoundHeroCard();
        }
    }
    private Pelicula GetRandomMovie(List<Pelicula> movies)
    {
        var random = new Random();
        return movies[random.Next(0, movies.Count)];
    }
    private HeroCard CreateHeroCard(Pelicula movie)
    {
        var imageUrl = Urls.GetImageUrl(movie.PosterPath);
        return new HeroCard
        {
            Title = movie.Title,
            Images = new List<CardImage> { new CardImage(imageUrl) }
        };
    }
    private HeroCard CreateNoMoviesFoundHeroCard()
    {
        return new HeroCard { Title = "No se encontraron películas" };
    }
    private async Task<DialogTurnResult> EndDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        return await stepContext.ReplaceDialogAsync(nameof(RootDialog), null, cancellationToken);
    }


}

