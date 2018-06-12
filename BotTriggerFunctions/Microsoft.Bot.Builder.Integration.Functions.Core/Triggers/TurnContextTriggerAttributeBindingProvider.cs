using Microsoft.Azure.WebJobs.Extensions.Bindings;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Bot.Builder.Integration.Functions.Core.Triggers
{
    internal class TurnContextTriggerAttributeBindingProvider
        : ITriggerBindingProvider
    {
        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParameterInfo parameter = context.Parameter;
            BotTriggerAttribute attribute = parameter.GetCustomAttribute<BotTriggerAttribute>(inherit: false);
            if (attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            // TODO: Define the types your binding supports here
            if (parameter.ParameterType != typeof(ITurnContext))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Can't bind SampleTriggerAttribute to type '{0}'.", parameter.ParameterType));
            }

            return Task.FromResult<ITriggerBinding>(new TurnContextTriggerBinding(context.Parameter));
        }

        private class TurnContextTriggerBinding
            : ITriggerBinding
        {
            private readonly ParameterInfo parameter;

            public TurnContextTriggerBinding(ParameterInfo parameter)
            {
                this.parameter = parameter;
                BindingDataContract = CreateBindingDataContract();
            }

            public IReadOnlyDictionary<string, Type> BindingDataContract { get; }

            public Type TriggerValueType
            {
                get { return typeof(ITurnContext); }
            }

            public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
            {
                // TODO: Perform any required conversions on the value
                // E.g. convert from Dashboard invoke string to our trigger
                // value type
                ITurnContext triggerValue = value as ITurnContext;
                IValueBinder valueBinder = new TurnContextValueBinder(parameter, triggerValue);
                return Task.FromResult<ITriggerData>(new TriggerData(valueBinder, GetBindingData(triggerValue)));
            }

            public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
            {
                return Task.FromResult<IListener>(new Listener(context.Executor));
            }

            public ParameterDescriptor ToParameterDescriptor()
            {
                return new TurnContextTriggerParameterDescriptor
                {
                    Name = parameter.Name,
                    DisplayHints = new ParameterDisplayHints
                    {
                        // TODO: Customize your Dashboard display strings
                        Prompt = "Sample",
                        Description = "Sample trigger fired",
                        DefaultValue = "Sample"
                    }
                };
            }

            private IReadOnlyDictionary<string, object> GetBindingData(ITurnContext value)
            {
                Dictionary<string, object> bindingData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                bindingData.Add("SampleTrigger", value);

                // TODO: Add any additional binding data
                return bindingData;
            }

            private IReadOnlyDictionary<string, Type> CreateBindingDataContract()
            {
                Dictionary<string, Type> contract = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
                contract.Add("SampleTrigger", typeof(ITurnContext));

                // TODO: Add any additional binding contract members
                return contract;
            }

            private class TurnContextTriggerParameterDescriptor : TriggerParameterDescriptor
            {
                public override string GetTriggerReason(IDictionary<string, string> arguments)
                {
                    // TODO: Customize your Dashboard display string
                    return string.Format("Sample trigger fired at {0}", DateTime.Now.ToString("o"));
                }
            }

            private class TurnContextValueBinder
                : ValueBinder
            {
                private readonly object _value;

                public TurnContextValueBinder(ParameterInfo parameter, ITurnContext value)
                    : base(parameter.ParameterType)
                {
                    _value = value;
                }

                public override Task<object> GetValueAsync()
                {
                    // TODO: Perform any required conversions
                    if (Type == typeof(string))
                    {
                        return Task.FromResult<object>(_value.ToString());
                    }
                    return Task.FromResult(_value);
                }

                public override string ToInvokeString()
                {
                    // TODO: Customize your Dashboard invoke string
                    return "Sample";
                }
            }

            private class Listener
                : IListener
            {
                private ITriggeredFunctionExecutor executor;
                private System.Timers.Timer _timer;

                public Listener(ITriggeredFunctionExecutor executor)
                {
                    this.executor = executor;

                    // TODO: For this sample, we're using a timer to generate
                    // trigger events. You'll replace this with your event source.
                    _timer = new System.Timers.Timer(5 * 1000)
                    {
                        AutoReset = true
                    };
                    _timer.Elapsed += OnTimer;
                }

                public Task StartAsync(CancellationToken cancellationToken)
                {
                    // TODO: Start monitoring your event source
                    _timer.Start();
                    return Task.FromResult(true);
                }

                public Task StopAsync(CancellationToken cancellationToken)
                {
                    // TODO: Stop monitoring your event source
                    _timer.Stop();
                    return Task.FromResult(true);
                }

                public void Dispose()
                {
                    // TODO: Perform any final cleanup
                    _timer.Dispose();
                }

                public void Cancel()
                {
                    // TODO: cancel any outstanding tasks initiated by this listener
                }

                private void OnTimer(object sender, System.Timers.ElapsedEventArgs e)
                {
                    // TODO: When you receive new events from your event source,
                    // invoke the function executor
                    TriggeredFunctionData input = new TriggeredFunctionData
                    {
                        TriggerValue = default(TurnContext),
                    };
                    executor.TryExecuteAsync(input, CancellationToken.None).Wait();
                }
            }
        }
    }
}
