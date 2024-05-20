using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly HttpClient _httpClient;
        private string apiKey = "a45cc75d9a7cde74a34d466138e3dd6d";

        public EchoBot(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            string url = $"https://api.themoviedb.org/3/discover/movie?api_key={apiKey}&with_genres=28";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            var movie = json["results"][0];

            var replyText = $"Echo: {turnContext.Activity.Text}\n\nHere is an action movie:\n";
            var posterPath = (string)movie["poster_path"];
            if (!string.IsNullOrEmpty(posterPath))
            {
                replyText += $"- {movie["title"]}\n";
                replyText += $"![{movie["title"]}](https://image.tmdb.org/t/p/w300_and_h450_bestv2/{posterPath})";
            }
            else
            {
                replyText += "No poster available for this movie.";
            }

            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }


        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
