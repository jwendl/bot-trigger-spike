using ExampleFunctions.Bots;
using ExampleFunctions.Interfaces;
using ExampleFunctions.Models;
using ExampleFunctions.Services;
using ExampleFunctions.State;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.Functions.Core.Extensions;
using Microsoft.Bot.Builder.Integration.Functions.Core.Triggers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ExampleFunctions
{
    public static class SimpleExample
    {
        private static readonly IServiceProvider serviceProvider;

        static SimpleExample()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ISimpleBot, SimpleBot>();
            serviceCollection.AddSingleton<IDataService<Person>, DataService<Person>>();

            serviceCollection.AddBot<SimpleBot, SimpleBotState>();
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [FunctionName("SimpleExample")]
        public static async Task RunAsync([BotTrigger]ITurnContext turnContext, TraceWriter log)
        {
            await turnContext.SendActivity("Hello World!");
        }
    }
}
