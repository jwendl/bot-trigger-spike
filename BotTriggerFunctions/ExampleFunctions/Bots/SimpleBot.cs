using ExampleFunctions.Interfaces;
using ExampleFunctions.Models;
using ExampleFunctions.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using System.Threading.Tasks;

namespace ExampleFunctions.Bots
{
    public class SimpleBot
        : ISimpleBot
    {
        private readonly IDataService<Person> dataService;

        public SimpleBot(IDataService<Person> dataService)
        {
            this.dataService = dataService;
        }

        public async Task OnTurn(ITurnContext turnContext)
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var simpleBotState = turnContext.GetConversationState<SimpleBotState>();
                simpleBotState.TurnNumber++;

                // calculate something for us to return
                int length = (turnContext.Activity.Text ?? string.Empty).Length;

                // simulate calling a dependent service that was injected
                var people = await dataService.FetchAllAsync();

                // return our reply to the user
                await turnContext.SendActivity($"[{simpleBotState.TurnNumber}] You sent {turnContext.Activity.Text} which was {length} characters");
            }

            if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                foreach (var newMember in turnContext.Activity.MembersAdded)
                {
                    if (newMember.Id != turnContext.Activity.Recipient.Id)
                    {
                        await turnContext.SendActivity("Hello and welcome to the echo bot.");
                    }
                }
            }
        }
    }
}
