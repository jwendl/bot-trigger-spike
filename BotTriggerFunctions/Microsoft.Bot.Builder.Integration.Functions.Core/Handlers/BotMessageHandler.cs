using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Bot.Builder.Integration.Functions.Core.Handlers
{
    public class BotMessageHandler
        : BotMessageHandlerBase
    {
        protected override async Task<InvokeResponse> ProcessMessageRequestAsync(HttpRequest request, BotFrameworkAdapter botFrameworkAdapter, Func<ITurnContext, Task> botCallbackHandler)
        {
            var activity = default(Activity);

            using (var bodyReader = new JsonTextReader(new StreamReader(request.Body, Encoding.UTF8)))
            {
                activity = BotMessageSerializer.Deserialize<Activity>(bodyReader);
            }

            var invokeResponse = await botFrameworkAdapter.ProcessActivity(request.Headers["Authorization"], activity, botCallbackHandler);
            return invokeResponse;
        }
    }
}
