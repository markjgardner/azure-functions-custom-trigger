using System;
using Microsoft.Azure.WebJobs.Host.Config;
using KafkaExtension.Trigger;
using Microsoft.Extensions.Logging;

namespace KafkaExtension 
{
  internal class KafkaExtensionConfigProvider : IExtensionConfigProvider
  {
    private readonly KafkaTriggerAttributeBindingProvider _trigger;
    public KafkaExtensionConfigProvider(KafkaTriggerAttributeBindingProvider trigger)
    {
      _trigger = trigger;
    }
    public void Initialize(ExtensionConfigContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context");
      }

      context.AddBindingRule<KafkaTriggerAttribute>()
        .BindToTrigger(_trigger);
    }
  }
}