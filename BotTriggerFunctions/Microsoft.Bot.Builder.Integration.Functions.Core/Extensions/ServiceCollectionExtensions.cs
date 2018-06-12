using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Integration.Functions.Core.Options;
using Microsoft.Bot.Builder.Integration.Functions.Core.Providers;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest.TransientFaultHandling;
using System;
using System.Net.Http;

namespace Microsoft.Bot.Builder.Integration.Functions.Core.Extensions
{
    /// <summary>
    /// Extension class for bot integration with ASP.NET Core 2.0 projects.
    /// </summary>
    /// <seealso cref="ApplicationBuilderExtensions"/>
    /// <seealso cref="BotAdapter"/>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures services for a <typeparamref name="TBot">specified bot type</typeparamref> to the <see cref="IServiceCollection" />.
        /// </summary>
        /// <typeparam name="TBot">A concrete type of <see cref="IBot"/ > that is to be registered and exposed to the Bot Framework.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configureAction">A callback that can further be used to configure the bot.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddBot<TBot, TBotState>(this IServiceCollection services)
            where TBot : class, IBot
            where TBotState : class, new()
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<IBot, TBot>();

            services.AddSingleton(sp =>
            {
                var environmentVariables = Environment.GetEnvironmentVariables();
                var options = new BotFrameworkOptions()
                {
                    CredentialProvider = new FunctionsCredentialProvider(environmentVariables),
                    ConnectorClientRetryPolicy = new RetryPolicy(new BotFrameworkHttpStatusCodeErrorDetectionStrategy(), 3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(1)),
                    HttpClient = new HttpClient(),
                };

                options.Middleware.Add(new CatchExceptionMiddleware<Exception>(async (context, exception) =>
                {
                    await context.TraceActivity("EchoBot Exception", exception);
                    await context.SendActivity("Sorry, it looks like something went wrong!");
                }));

                var memoryStore = new MemoryStorage();
                options.Middleware.Add(new ConversationState<TBotState>(memoryStore));

                var botFrameworkAdapter = new BotFrameworkAdapter(options.CredentialProvider, options.ConnectorClientRetryPolicy, options.HttpClient);
                foreach (var middleware in options.Middleware)
                {
                    botFrameworkAdapter.Use(middleware);
                }

                return botFrameworkAdapter;
            });

            return services;
        }
    }
}
