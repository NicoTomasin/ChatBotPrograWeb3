using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BotPeliculas.Bot.Dialogs;
using BotPeliculas.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<JObject> _userStateAccessor;
        private readonly IPeliculasService _peliculasService;
        public RootDialog(UserState userState, IPeliculasService peliculasService)
        : base(nameof(RootDialog))
        { 
            _userStateAccessor = userState.CreateProperty<JObject>("result");
            AddDialog(new TextPrompt("text"));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] { ShowOptionsAsync, HandleOptionAsync }));
            AddDialog(new Top10(peliculasService));
            AddDialog(new RecomendarPelicula(peliculasService));
            AddDialog(new ProximasPeliculas(peliculasService));
            InitialDialogId = nameof(WaterfallDialog);
            _peliculasService = peliculasService;
        }

        private async Task<DialogTurnResult> ShowOptionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard
            {
                Title = "Elige una opción",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, title: "Ver peliculas del dia", value: "Top10"),
                    new CardAction(ActionTypes.ImBack, title: "Recomendar pelicula", value: "RecomendarPelicula"),
                    new CardAction(ActionTypes.ImBack, title: "Próximas películas", value: "ProximasPeliculas")
                }
            };

            var reply = MessageFactory.Attachment(card.ToAttachment());
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return Dialog.EndOfTurn;
        }

        private async Task<DialogTurnResult> HandleOptionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choice = stepContext.Context.Activity.Text;

            try
            {
                return await stepContext.BeginDialogAsync(choice, null, cancellationToken);
            }
            catch (Exception)
            {
                await stepContext.Context.SendActivityAsync("Opción inexistente", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(InitialDialogId, null, cancellationToken);
            }
        }
    }
}
