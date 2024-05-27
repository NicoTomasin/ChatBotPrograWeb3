using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<JObject> _userStateAccessor;

        public RootDialog(UserState userState)
            : base("root")
        {
            _userStateAccessor = userState.CreateProperty<JObject>("result");
            AddDialog(new TextPrompt("text"));
            AddDialog(new WaterfallDialog("waterfall", new WaterfallStep[] { ShowOptionsAsync, HandleOptionAsync }));
            AddDialog(new Top10());
            AddDialog(new RecomendarPelicula());
            InitialDialogId = "waterfall";
        }

        private async Task<DialogTurnResult> ShowOptionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard
            {
                Title = "Elige una opción",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, title: "Ver peliculas del dia", value: "Top10"),
                    new CardAction(ActionTypes.ImBack, title: "Recomendar pelicula", value: "RecomendarPelicula")
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
            catch (Exception ex)
            {
                await stepContext.Context.SendActivityAsync("Opción inexistente", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(stepContext.ActiveDialog.Id, null, cancellationToken);
            }
        }

    }
}
