using Microsoft.Bot.Connector.Authentication;
using System.Collections;

namespace Microsoft.Bot.Builder.Integration.Functions.Core.Providers
{
    public sealed class FunctionsCredentialProvider
        : SimpleCredentialProvider
    {
        public FunctionsCredentialProvider(IDictionary environmentVariables)
        {
            AppId = environmentVariables[MicrosoftAppCredentials.MicrosoftAppIdKey]?.ToString();
            Password = environmentVariables[MicrosoftAppCredentials.MicrosoftAppPasswordKey]?.ToString();
        }
    }
}
