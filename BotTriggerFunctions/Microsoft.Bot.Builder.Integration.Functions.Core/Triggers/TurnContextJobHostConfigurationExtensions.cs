using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using System;

namespace Microsoft.Bot.Builder.Integration.Functions.Core.Triggers
{
    public static class TurnContextJobHostConfigurationExtensions
    {
        public static void UseTurnContext(this JobHostConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            // Register our extension configuration provider
            config.RegisterExtensionConfigProvider(new TurnContextExtensionConfig());
        }

        private class TurnContextExtensionConfig
            : IExtensionConfigProvider
        {
            public void Initialize(ExtensionConfigContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                // Register our extension binding providers
                context.Config.RegisterBindingExtensions(new TurnContextTriggerAttributeBindingProvider());
            }
        }
    }
}
