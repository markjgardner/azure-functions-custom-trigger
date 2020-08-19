using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace KafkaExtension.Trigger
{
  internal class KafkaTriggerParameterDescriptor : TriggerParameterDescriptor
  {
    public string TopicName { get; set; }
    public string GroupId { get; set; }

    public override string GetTriggerReason(IDictionary<string, string> arguments) 
    {
      return string.Format(KafkaTriggerConstants.TriggerDescription, this.TopicName, this.GroupId, DateTime.UtcNow.ToString("o"));
    }
  }
}