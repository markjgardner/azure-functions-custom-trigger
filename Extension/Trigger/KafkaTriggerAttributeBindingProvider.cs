using System;
using System.Reflection;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KafkaExtension.Trigger 
{
  internal class KafkaTriggerAttributeBindingProvider : ITriggerBindingProvider 
  {
    private readonly ILogger _logger;
    public KafkaTriggerAttributeBindingProvider (ILoggerFactory loggerFactory) 
    {
      _logger = loggerFactory.CreateLogger (LogCategories.CreateTriggerCategory ("Kafka"));
    }

    public Task<ITriggerBinding> TryCreateAsync (TriggerBindingProviderContext context) 
    {
      if (context == null) {
        throw new ArgumentNullException ("context");
      }

      ParameterInfo parameter = context.Parameter;
      KafkaTriggerAttribute attribute = parameter.GetCustomAttribute<KafkaTriggerAttribute> (inherit: false);
      if (attribute == null) 
      {
        return Task.FromResult<ITriggerBinding>(null);
      }

      var config = new ConsumerConfig() {
        GroupId = attribute.GroupId,
        BootstrapServers = attribute.BootstrapServers
      };

      var topic = attribute.TopicName;

      return Task.FromResult<ITriggerBinding>(new KafkaTriggerBinding(parameter, config, topic, _logger));
    }
  }
}