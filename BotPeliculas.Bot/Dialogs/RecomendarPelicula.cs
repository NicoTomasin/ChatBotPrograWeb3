using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System;
using Microsoft.BotBuilderSamples;
using BotPeliculas.Interfaces;

public class RecomendarPelicula : ComponentDialog
{
    private static readonly Dictionary<string, string> MoodToGenre = new Dictionary<string, string>
    {
        { "Triste", "18" },
        { "Feliz", "35" },
        { "Enojado", "28" },
        { "Romantico", "10749" },
        { "Nervioso", "27" },
        { "Curioso", "99" }
    };
    private (string Mood, string Genre) userSelection;
    private readonly IPeliculasService _peliculasService;
    public RecomendarPelicula(IPeliculasService peliculasService) : base(nameof(RecomendarPelicula))
    {
        var waterfallSteps = new WaterfallStep[]
        {
            AskForMoodStepAsync,
            HandleMoodSelectionStepAsync,
            ShowOptionsStepAsync,
            HandleOptionSelectionStepAsync,
            EndDialogAsync
        };

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
        AddDialog(new Top10(peliculasService));
        AddDialog(new MasVista(peliculasService));
        AddDialog(new PeliculaRandom(peliculasService));
        InitialDialogId = nameof(WaterfallDialog);
        _peliculasService = peliculasService;
    }

    private async Task<DialogTurnResult> AskForMoodStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var options = new List<string>(MoodToGenre.Keys);
        var promptOptions = new PromptOptions
        {
            Prompt = MessageFactory.Text("Seleccione un estado de ánimo:"),
            Choices = ChoiceFactory.ToChoices(options),
            Style = ListStyle.HeroCard
        };
        return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
    }

    private async Task<DialogTurnResult> HandleMoodSelectionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var moodChoice = ((FoundChoice)stepContext.Result).Value;
        if (MoodToGenre.TryGetValue(moodChoice, out var genre))
        {
            userSelection = (moodChoice, genre);
        }
        return await stepContext.NextAsync(null, cancellationToken);
    }

    private async Task<DialogTurnResult> ShowOptionsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var card = new HeroCard
        {
            Title = $"Seleccione qué desea para {userSelection.Mood}:",
            Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, title: "Top 10 más vista", value: "Top10"),
                    new CardAction(ActionTypes.ImBack, title: "Ver la más vista", value: "MasVista"),
                    new CardAction(ActionTypes.ImBack, title: "Película Random", value: "PeliculaRandom")
                }
        };

        var reply = MessageFactory.Attachment(card.ToAttachment());
        await stepContext.Context.SendActivityAsync(reply, cancellationToken);
        return Dialog.EndOfTurn;
    }

    private async Task<DialogTurnResult> HandleOptionSelectionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var choice = stepContext.Context.Activity.Text;

        try
        {
            return await stepContext.BeginDialogAsync(choice, userSelection, cancellationToken);
        }
        catch (Exception ex)
        {
            await stepContext.Context.SendActivityAsync("Opción inexistente", cancellationToken: cancellationToken);
            return await stepContext.ReplaceDialogAsync(stepContext.ActiveDialog.Id, null, cancellationToken);
        }
    }
    private async Task<DialogTurnResult> EndDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        return await stepContext.ReplaceDialogAsync(nameof(RootDialog), null, cancellationToken);
    }
}
